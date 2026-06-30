using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.ApiPayloadModels;
using Pipelines.Sockets.Unofficial.Arenas;
using MimeKit.Utils;
using ContractorsDesk.Core.Utilities;
using DocuSign.eSign.Model;

namespace ContractorsDesk.Services
{
	public class RevisionsService : BaseService, IRevisionsService
	{
		public RevisionsService(
			ClientDbContext clientDbContext,
			IMapper mapper)
			: base(mapper, clientDbContext)
		{
		}

		public async Task<List<ScheduleRevisionDto>> GetScheduleRevisionsByProjectIdAsync(Guid projectId, int? statusId = null)
		{
			var scheduleRevisions = await ClientDbContext.ScheduleRevisions
				.Where(r => r.ProjectId == projectId && (!statusId.HasValue || r.StatusId == statusId))
				.ToListAsync();

			return mapper.Map<List<ScheduleRevisionDto>>(scheduleRevisions);
		}
		public async Task<ScheduleRevisionDto> GetScheduleRevisionByIdAsync(Guid id)
		{
			var scheduleRevisions = await ClientDbContext.ScheduleRevisions
				.FirstOrDefaultAsync(r => r.Id == id);

			return mapper.Map<ScheduleRevisionDto>(scheduleRevisions);
		}

		public async Task<CostRevisionDto> CreateCostRevisionAsync(CostRevisionPayload costRevisionPayload)
		{
			//create cost revision
			var projectId = costRevisionPayload.ProjectId;
			var proposal = await ClientDbContext.Proposals.Include(p => p.ProposalLines).FirstOrDefaultAsync(p => p.QbclassId == projectId);

			var estimateCategory = proposal.ProposalLines.FirstOrDefault(pl => pl.EstimateCategoryId == costRevisionPayload.EstimateCategoryId);
			var latestProposalItemHistory = await ClientDbContext.ProposalLinesHistories
			.Where(p => p.ProposalId == proposal.Id && p.EstimateCategoryId == estimateCategory.EstimateCategoryId)
			.OrderByDescending(p => p.ChangeDate)
			.FirstOrDefaultAsync();

			var latestRevisionNumber = await ClientDbContext.CostRevisions
				.MaxAsync(sr => (int?)sr.RevisionNumber) ?? 0;

			latestRevisionNumber += 1;

			var costrevision = new CostRevision
			{
				Id = Guid.NewGuid(),
				RevisionNumber = latestRevisionNumber,
				RevisionDate = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				ActionItemId = costRevisionPayload.ActionItemId,
				//Amount = costRevisionPayload.Amount,
				//CurrentAmount = latestProposalItemHistory.Amount,
				//NewAmount = costRevisionPayload.Amount + latestProposalItemHistory.Amount,
				//EstimateCategoryId = costRevisionPayload.EstimateCategoryId,
				ProjectId = projectId,
				StatusId = (int)RevisionStatus.New
			};

			ClientDbContext.CostRevisions.Add(costrevision);
			await ClientDbContext.SaveChangesAsync();

			return await GetCostRevisionAsync(costrevision.Id);
		}

		public async Task<ScheduleRevisionDto> CreateScheduleRevisionsAsync(ScheduleRevisionPayload revision, Guid projectId)
		{
			return new ScheduleRevisionDto();
			//var project = await clientDbContext.Qbclasses
			//	.AsNoTracking()
			//	.Where(ps => ps.Id == projectId)
			//	.Include(p=> p.ProjectSchedules)
			//	.ThenInclude(ps => ps.ProjectScheduleTasks)
			//	.FirstOrDefaultAsync();

			//var latestRevisionNumber = await clientDbContext.ScheduleRevisions
			//	.MaxAsync(sr => (int?)sr.RevisionNumber) ?? 0;

			//latestRevisionNumber += 1;

			//var projectScheduleItems = project.ProjectSchedules.FirstOrDefault()?.ProjectScheduleTasks.ToList();

			//var projectScheduleItem = projectScheduleItems.FirstOrDefault(ps => ps.ConstructionTaskId == revision.ConstructionTaskId);

			//var dbRevision = new ScheduleRevision();
			//dbRevision.Id = Guid.NewGuid();
			//dbRevision.ProjectId = projectId;
			//dbRevision.ActionItemId = revision.ActionItemId;
			//dbRevision.RevisionNumber = latestRevisionNumber;
			//dbRevision.RevisionDate = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			//dbRevision.Reason = revision.Reason;
			//dbRevision.Description = revision.Description;
			//dbRevision.ConstructionTaskId = revision.ConstructionTaskId;
			//dbRevision.NewDuration = revision.NewDuration;
			//dbRevision.NewStartDate = revision.NewStartDate;
			//dbRevision.NewEndDate = revision.NewEndDate;
			//dbRevision.OldDuration = projectScheduleItem.Duration;
			//dbRevision.OldStartDate = projectScheduleItem.StartDate;
			//dbRevision.OldEndDate = projectScheduleItem.EndDate;
			//dbRevision.StatusId = (int)RevisionStatus.New;
			//clientDbContext.ScheduleRevisions.Add(dbRevision);
			//await clientDbContext.SaveChangesAsync();

			//return mapper.Map<ScheduleRevisionDto>(dbRevision);
		}

		public async Task<CostRevisionDto> GetCostRevisionAsync(Guid id)
		{
			var dbCostRevision = await ClientDbContext
				.CostRevisions
				.Include(c=>c.CostRevisionItems)
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Id == id);

			var retval = mapper.Map<CostRevisionDto>(dbCostRevision);

			var proposal = await ClientDbContext.Proposals.Include(p => p.ProposalLines).FirstOrDefaultAsync(p => p.QbclassId == dbCostRevision.ProjectId);

			if (proposal != null)
			{
				foreach(var item in retval.CostRevisionItems)
				{
					var estimateCategory = proposal.ProposalLines.FirstOrDefault(pl => pl.EstimateCategoryId == item.EstimateCategoryId);
					if (estimateCategory != null)
					{
						item.EstimateCategory = estimateCategory.Name;
					}
				}
			}

			return retval;
		}

		public async Task MarkScheduleRevisionAsSent(Guid scheduleRevisionId)
		{
			var dbScheduleRevision = await ClientDbContext.ScheduleRevisions.FirstOrDefaultAsync(sr=> sr.Id == scheduleRevisionId);

			if (dbScheduleRevision != null)
			{
				dbScheduleRevision.StatusId = (int)RevisionStatus.Sent;
				await ClientDbContext.SaveChangesAsync();
			}
		}

		public async Task AssignActionItemToScheduleRevision(Guid scheduleRevisionId, int actionItemId)
		{
			var scheduleRevision = await ClientDbContext.ScheduleRevisions.FirstOrDefaultAsync(s=> s.Id == scheduleRevisionId);
			if (scheduleRevision != null)
			{
				scheduleRevision.ActionItemId = actionItemId;
				await ClientDbContext.SaveChangesAsync();
			}
		}

		public async Task AssignActionItemToCostRevision(Guid costRevisionId, int actionItemId)
		{
			var scheduleRevision = await ClientDbContext.CostRevisions.FirstOrDefaultAsync(s => s.Id == costRevisionId);
			if (scheduleRevision != null)
			{
				scheduleRevision.ActionItemId = actionItemId;
				await ClientDbContext.SaveChangesAsync();
			}
		}

	}
}
