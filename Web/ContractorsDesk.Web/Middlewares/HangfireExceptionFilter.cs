using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class HangfireExceptionFilter : JobFilterAttribute, IServerFilter, IApplyStateFilter
	{
		private readonly string telegramBotApi = "https://api.telegram.org/bot";
		private readonly string botToken = "7747813576:AAHNFnrvhjgCI_5ha8NsRnA-5vhWNY72ZVA"; // Telegram Bot Token - ContractorsDesk - Data Sync
		private readonly string chatId = "-1002557849513"; // Telegram Bot User - contractorsdesk_datasyncBot
		private readonly ITelegramService telegramService;
		public HangfireExceptionFilter(ITelegramService telegramService)
		{
			this.telegramService = telegramService;
		}
		public void OnPerforming(PerformingContext context)
		{
			var job = context.BackgroundJob;
			var jobName = $"{job.Job.Type.Name}.{job.Job.Method.Name}";
			var jobId = job.Id;

			var message = $"🚀 *Job Starting*" +
						  $"\n\n🆔 `{jobId}`" +
						  $"\n🧾 `{jobName}`";

			_ = telegramService.SendMessageAsync(message);
		}

		public void OnPerformed(PerformedContext context)
		{
			var job = context.BackgroundJob;
			var jobName = $"{job.Job.Type.Name}.{job.Job.Method.Name}";
			var jobId = job.Id;
			var succeeded = context.Exception == null;

			var status = succeeded ? $"✅ *Job Completed Successfully*" : "⚠️ *Job Completed With Errors*";
			var message = $"{status}" +
						  $"\n\n🆔 `{jobId}`" +
						  $"\n🧾 `{jobName}`";

			if (context.Exception != null)
			{
				var innerExeption = context.Exception.InnerException;
				var errorMessage = innerExeption != null ? innerExeption.Message : context.Exception.Message;

				if (innerExeption?.InnerException != null)
					errorMessage = innerExeption.InnerException.Message;

				message += $"\n💥 Error: `{EscapeMarkdown(errorMessage)}`";

				if (context.Exception?.StackTrace != null)
				{
					var stackTrace = context.Exception.InnerException != null ? context.Exception.InnerException.StackTrace : context.Exception?.StackTrace;
					message += $"\n🧵 StackTrace: `{EscapeMarkdown(stackTrace)}`";
				}
			}

			_ = telegramService.SendMessageAsync(message);
		}

		public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			var job = context.BackgroundJob;
			var jobName = $"{job.Job.Type.Name}.{job.Job.Method.Name}";
			var jobId = job.Id;

			switch (context.NewState)
			{
				case EnqueuedState:
					_ = telegramService.SendMessageAsync($"📥 *Job Enqueued*\n\n🆔 `{jobId}`\n🧾 `{jobName}`");
					break;

				case SucceededState:
					_ = telegramService.SendMessageAsync($"✅ *Job Succeeded*\n\n🆔 `{jobId}`\n🧾 `{jobName}`");
					break;

				case FailedState failedState:
					var exception = failedState.Exception;
					var message = $"🚨 *Job Failed!* 🚨\n\n" +
								  $"🆔 `{jobId}`\n" +
								  $"🧾 `{jobName}`\n" +
								  $"💥 `{EscapeMarkdown(exception?.Message ?? "No message")}`\n" +
								  $"🧵 `{EscapeMarkdown(exception?.StackTrace ?? "No stack trace")}`";
					_ = telegramService.SendMessageAsync(message);
					break;
			}
		}

		public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			var jobId = context.BackgroundJob.Id;
			var oldState = context.OldStateName;
			var newState = context.NewState?.GetType().Name ?? "Unknown";

			if (context.BackgroundJob.Job != null)
			{
				var jobName = $"{context.BackgroundJob.Job.Type.Name}.{context.BackgroundJob.Job.Method.Name}";
				var message = $"🔄 *Job State Transition *\n\n" +
							  $"🆔 `{jobId}`\n" +
							  $"🧾 `{jobName}`\n" +
							  $"🔙 From: `{EscapeMarkdown(oldState)}`\n" +
							  $"➡️ To: `{EscapeMarkdown(newState)}`";

				_ = telegramService.SendMessageAsync(message);
			}
		}

		private string EscapeMarkdown(string text)
		{
			if (string.IsNullOrEmpty(text))
				return "";

			return text
				.Replace("_", "\\_")
				.Replace("*", "\\*")
				.Replace("[", "\\[")
				.Replace("]", "\\]")
				.Replace("(", "\\(")
				.Replace(")", "\\)")
				.Replace("~", "\\~")
				.Replace("`", "\\`")
				.Replace(">", "\\>")
				.Replace("#", "\\#")
				.Replace("+", "\\+")
				.Replace("-", "\\-")
				.Replace("=", "\\=")
				.Replace("|", "\\|")
				.Replace("{", "\\{")
				.Replace("}", "\\}")
				.Replace(".", "\\.")
				.Replace("!", "\\!");
		}
	}
}
