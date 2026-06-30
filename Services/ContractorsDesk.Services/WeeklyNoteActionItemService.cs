using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ContractorsDesk.Services
{
	public class WeeklyNoteActionItemService : BaseService, IWeeklyNoteActionItemService
	{
		private readonly IPostMarkEmailService emailService;
		private List<DataStore.Client.Models.User> CompanyOwners { get; set; } = new List<DataStore.Client.Models.User>();

		public WeeklyNoteActionItemService(ClientDbContext clientDbContext,
			IConfiguration configuration,
			IPostMarkEmailService emailService,
			INotificationService notificationService)
            : base (mapper: null, clientDataDbContext: clientDbContext, configuration: configuration, notificationService: notificationService)
        {
			this.emailService = emailService;
		}

		[AutomaticRetry(Attempts = 0)]
		public async Task CreateWeeklyNoteActionItem(SignalRMessageModel? signalRMessageModel = null)
        {
			try
			{
				this.signalRMessageModel = signalRMessageModel;
				var weeklyJobsProjectTasks = await GenerateWeeklyJobPayloadsAsync();

				await GetCompanyOwners();

				foreach (var weeklyJobsProjectTask in weeklyJobsProjectTasks)
				{
					var emailModel = new EmailPayloadModel
					{
						Subject = $"{configuration["GmailSmtpServer:AppName"] ?? string.Empty} - Weekly Construction Tasks:",
						Body = GenerateEmailBody(new List<WeeklyJobEmailToSupervisorDto>() { weeklyJobsProjectTask }, false),
						SaveToDatabase = false
					};
					
					if (weeklyJobsProjectTask.Supervisors.Any())
					{
						await SendWeeklyTaskEmail(weeklyJobsProjectTask.Supervisors, emailModel, false);
					}
				}

				await SendSummaryToCompanyOwners(weeklyJobsProjectTasks);
				await SendCompletedStatusNotification();
			}
			catch (Exception ex)
			{
				await RaiseErrorNotification(ex.Message, "Weekly Run Task Action Item!");
			}
		}

		private async Task SendSummaryToCompanyOwners(List<WeeklyJobEmailToSupervisorDto> weeklyJobsProjectTasks)
		{
			var emailModel = new EmailPayloadModel
			{
				Subject = $"{configuration["GmailSmtpServer:AppName"] ?? string.Empty} - Weekly Project Task Schedule:",
				Body = GenerateEmailBody(weeklyJobsProjectTasks, true),
				SaveToDatabase = false
			};

			var companyOwners = CompanyOwners
				.Select(u => new WeeklyJobSupervisorsDto
				{
					UserId = u.Id,
					SupervisorsName = $"{u.FirstName} {u.LastName}",
					SupervisorsEmail = u.Email
				})
				.ToList();

			await SendWeeklyTaskEmail(companyOwners, emailModel, true);
		}

		private async Task SendWeeklyTaskEmail(List<WeeklyJobSupervisorsDto> supervisors, EmailPayloadModel emailModel, bool isSummary = false)
		{
			foreach (var supervisor in supervisors)
			{
				if (!isSummary && CompanyOwners.Any(co => co.Id == supervisor.UserId))
					continue;

				emailModel.To = supervisor.SupervisorsEmail;

				var emailSent = await emailService.SendEmailAsync(emailModel);
			}
		}

		private async Task GetCompanyOwners()
		{
			CompanyOwners = await ClientDbContext.Users
				.Where(u => u.Role != null && u.Role.Name == "Company Owner")
				.ToListAsync();
		}

		private async Task SendCompletedStatusNotification()
		{
			var message = $"\n💯 *Weekly Run Task Action Item Completed Successfully*\n";

			if (this.signalRMessageModel != null && this.signalRMessageModel.NotificationId != null)
			{
				var userNotification = await ClientDbContext.UserNotifications
					.FirstOrDefaultAsync(n => n.Id == this.signalRMessageModel.NotificationId);

				ClientDbContext.UserNotifications.Remove(userNotification);
				await ClientDbContext.SaveChangesAsync();
			}

			await SendNotification(message);
		}

		private async Task<List<WeeklyJobEmailToSupervisorDto>> GenerateWeeklyJobPayloadsAsync()
		{
			var projectScheduleSupervisors = await GetProjectScheduleSupervisorQuery()
				.Distinct()
				.ToListAsync();

			var projectSchedules = GroupProjectSchedules(projectScheduleSupervisors);
			var (monday, friday) = GetCurrentWeekRange();

			var payloads = new List<WeeklyJobEmailToSupervisorDto>();

			foreach (var projectSchedule in projectSchedules)
			{
				var tasks = await GetTasksForScheduleWithinWeekAsync(projectSchedule.Key.ProjectScheduleId, monday, friday);

				if (tasks.Any())
				{
					var weeklyJob = await GetWeeklyJob(projectSchedule, tasks, monday, friday);

					payloads.Add(weeklyJob);
				}
			}

			return payloads;
		}

		private IQueryable<ProjectScheduleSupervisorDto> GetProjectScheduleSupervisorQuery()
		{
			return from ps in ClientDbContext.ProjectSchedules
				   join qbc in ClientDbContext.Qbclasses
					   on ps.ProjectId equals qbc.Id into qbcJoin
				   from qbc in qbcJoin.DefaultIfEmpty()
				   join psvrs in ClientDbContext.ProjectSupervisors
					   on ps.ProjectId equals psvrs.ProjectId into psvrsJoin
				   from psvrs in psvrsJoin.DefaultIfEmpty()
				   where qbc != null 
				   && qbc.IsArchived != true && qbc.IsDeleted != true
				   && (qbc.ActiveJobs == true || qbc.ActiveSpecJobs == true)
				   select new ProjectScheduleSupervisorDto
				   {
					   ProjectId = qbc.Id,
					   ProjectName = qbc.Name ?? string.Empty,
					   ProjectScheduleId = ps.Id,
					   ProjectSupervisorId = psvrs != null ? psvrs.SupervisorId : (int?)null
				   };
		}

		private static List<IGrouping<(Guid ProjectId, string ProjectName, Guid ProjectScheduleId), ProjectScheduleSupervisorDto>> GroupProjectSchedules(
			List<ProjectScheduleSupervisorDto> scheduleSupervisors)
		{
			return scheduleSupervisors
				.GroupBy(x => (x.ProjectId, x.ProjectName, x.ProjectScheduleId))
				.ToList();
		}

		private static (DateOnly Monday, DateOnly Friday) GetCurrentWeekRange()
		{
			var now = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			int daysToMonday = ((int)now.DayOfWeek + 6) % 7;
			var monday = now.AddDays(-daysToMonday).Date;
			var friday = monday.AddDays(4).Date;

			return (DateOnly.FromDateTime(monday), DateOnly.FromDateTime(friday));
		}

		private async Task<List<(string ProjectTaskname, DateOnly? StartDate, DateOnly? EndDate)>> GetTasksForScheduleWithinWeekAsync(
			Guid projectScheduleId, DateOnly monday, DateOnly friday)
		{
			return await ClientDbContext.ProjectScheduleTasks
				.Where(task => task.ProjectScheduleId == projectScheduleId &&
							   task.StartDate >= monday &&
							   task.StartDate <= friday)
				.Select(task => new ValueTuple<string, DateOnly?, DateOnly?>(
					task.Name,
					task.StartDate,
					task.EndDate))
				.ToListAsync();
		}

		private async Task<WeeklyJobEmailToSupervisorDto> GetWeeklyJob(
			IEnumerable<ProjectScheduleSupervisorDto> projectSchedules, List<(string projectTaskname, DateOnly? startDate, DateOnly? endDate)> tasks,
			DateOnly weekStartDate, DateOnly weekEndDate)
		{
			var projectSchedule = projectSchedules.FirstOrDefault();
			var endDate = tasks
				.Where(t => t.endDate.HasValue)
				.Select(t => t.endDate.Value)
				.DefaultIfEmpty(weekEndDate)
				.Max();
			var title = $"{projectSchedule?.ProjectName}: Construction tasks to work on for the week {weekStartDate} - {weekEndDate}";
			var description = tasks.OrderBy(t => t.startDate ?? DateOnly.MinValue)
					.ThenBy(t => t.endDate ?? DateOnly.MinValue)
					.Select(t => $"{t.projectTaskname} \t {t.startDate?.ToString("yyyy-MM-dd")} \t {t.endDate?.ToString("yyyy-MM-dd")}")
					.ToList();

			var supervisors = await GetSupervisorEmails(projectSchedules);

			return new WeeklyJobEmailToSupervisorDto
			{
				Title = title,
				Descriptions = description,
				Supervisors = supervisors
			};
		}

		private async Task<List<WeeklyJobSupervisorsDto>> GetSupervisorEmails(IEnumerable<ProjectScheduleSupervisorDto> group)
		{
			var supervisorIds = group
				.Select(x => x.ProjectSupervisorId)
				.Where(id => id.HasValue)
				.Select(id => id.Value)
				.Distinct()
				.ToList();

			var supervisorEmails = await ClientDbContext.Users
				.Where(u => supervisorIds.Contains(u.Id))
				.Select(u => new WeeklyJobSupervisorsDto
				{
					UserId = u.Id,
					SupervisorsName = $"{u.FirstName} {u.LastName}",
					SupervisorsEmail = u.Email
				})
				.ToListAsync();


			return supervisorEmails;
		}

		private string GenerateEmailBody(List<WeeklyJobEmailToSupervisorDto> weeklyJobEmails, bool isSummary = false)
		{
			var sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html>");
			sb.AppendLine("<head>");
			sb.AppendLine("    <meta charset=\"UTF-8\">");
			sb.AppendLine("    <style>");
			sb.AppendLine("        html, body { padding: 0; margin: 0; font-family: Arial, Helvetica, sans-serif; background-color: #ffffff; }");
			sb.AppendLine("        .container { max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 6px; padding: 40px 20px; color: #2F3044; font-size: 15px; line-height: 1.5; }");
			sb.AppendLine("        .header { font-size: 20px; font-weight: bold; margin-bottom: 30px; text-align: left; }");
			sb.AppendLine("        .section-title { font-size: 17px; font-weight: bold; margin: 25px 0 10px; }");
			sb.AppendLine("        .section-manager { font-size: 16px; font-weight: bold; margin-bottom: 30px; text-align: left; }");
			sb.AppendLine("        table { width: 100%; border-collapse: collapse; margin-bottom: 25px; font-size: 14px; }");
			sb.AppendLine("        table thead { background-color: #f2f2f2; }");
			sb.AppendLine("        table th, table td { padding: 8px; border: 1px solid #ddd; text-align: left; }");
			sb.AppendLine("    </style>");
			sb.AppendLine("</head>");
			sb.AppendLine("<body>");
			sb.AppendLine("    <div class=\"container\">");

			if (isSummary)
			{
				sb.AppendLine("        <div class=\"header\">Weekly Project Task Schedule</div>");
				sb = GenerateEmailBodyForSummary(sb, weeklyJobEmails);
			}
			else
			{
				sb = GenerateEmailBodyForSupervisor(sb, weeklyJobEmails);
			}

			sb.AppendLine("    </div>");
			sb.AppendLine("</body>");
			sb.AppendLine("</html>");

			return sb.ToString();
		}

		private StringBuilder GenerateEmailBodyForSupervisor(StringBuilder sb, List<WeeklyJobEmailToSupervisorDto> weeklyJobEmails)
		{
			foreach (var weeklyJobEmail in weeklyJobEmails)
			{
				sb.AppendLine($"        <div class=\"section-title\">{weeklyJobEmail.Title}</div>");
				AppendTaskTable(sb, weeklyJobEmail.Descriptions);
			}

			return sb;
		}

		private StringBuilder GenerateEmailBodyForSummary(StringBuilder sb, List<WeeklyJobEmailToSupervisorDto> weeklyJobEmails)
		{
			var supervisorGroups = weeklyJobEmails
				.SelectMany(w => 
					w.Supervisors.Select(name => 
						new { SupervisorName = name.SupervisorsName, w.Title, w.Descriptions }
					)
				)
				.GroupBy(x => x.SupervisorName);

			foreach (var supervisorGroup in supervisorGroups)
			{
				sb.AppendLine("        <div style=\"border: 2px solid #ddd; border-radius: 6px; padding: 15px; margin-bottom: 20px;\">");
				sb.AppendLine($"            <div class=\"section-manager\">Project Manager: {supervisorGroup.Key}</div>");

				foreach (var project in supervisorGroup)
				{
					sb.AppendLine($"            <div class=\"section-title\">Project: {project.Title.Split(":")[0].Trim()}</div>");
					AppendTaskTable(sb, project.Descriptions);
				}

				sb.AppendLine("        </div>");
			}

			return sb;
		}

		private void AppendTaskTable(StringBuilder sb, List<string> descriptions)
		{
			sb.AppendLine("        <table>");
			sb.AppendLine("            <thead>");
			sb.AppendLine("                <tr>");
			sb.AppendLine("                     <th>Task</th>");
			sb.AppendLine("                     <th>Start Date</th>");
			sb.AppendLine("                     <th>End Date</th>");
			sb.AppendLine("                </tr>");
			sb.AppendLine("            </thead>");
			sb.AppendLine("            <tbody>");

			foreach (var description in descriptions)
			{
				var cols = description.Split("\t", StringSplitOptions.RemoveEmptyEntries);
				sb.AppendLine("                <tr>");
				sb.AppendLine($"                     <td>{cols.ElementAtOrDefault(0)?.Trim() ?? ""}</td>");
				sb.AppendLine($"                     <td>{cols.ElementAtOrDefault(1)?.Trim() ?? ""}</td>");
				sb.AppendLine($"                     <td>{cols.ElementAtOrDefault(2)?.Trim() ?? ""}</td>");
				sb.AppendLine("                </tr>");
			}

			sb.AppendLine("            </tbody>");
			sb.AppendLine("        </table>");
		}
	}
}
