using ContractorsDesk.Services.Interfaces.@base;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.@base
{
	public abstract class BaseService : IBaseService
	{
		public IMapper? mapper{ get; set; }
		public MasterDbContext? masterDbContext { get; set; }
		public IConfiguration? configuration { get; set; }

		public StringBuilder NotificationMessage = new StringBuilder();
		public SignalRMessageModel? signalRMessageModel = null;
		private readonly INotificationService? notificationService;
		private TelegramMessageModel CurrentTelegramMessage = new TelegramMessageModel();
		public BaseService() { }
        public BaseService(
            IMapper? mapper, 
            ClientDbContext? clientDataDbContext = null, 
			MasterDbContext? masterDbContext = null,
			IConfiguration? configuration = null,
			INotificationService? notificationService = null
			)
        {
            this.mapper = mapper; 
            this.ClientDbContext = clientDataDbContext; 
			this.masterDbContext = masterDbContext;
            this.configuration = configuration;
			this.notificationService = notificationService;
		}

		private ClientDbContext? clientDbContext;
		public ClientDbContext? ClientDbContext { get { return this.clientDbContext; } set { this.clientDbContext = value; } }
		public virtual Task<T> CreateAsync<T>(object param)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> UpdateAsync<T>(object param)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(object param)
        {
            throw new NotImplementedException();
        }
        public virtual Task<T> GetAllAsync<T>()
        {
            throw new NotImplementedException();
        }
		protected async Task SendNotification(string message)
		{
			this.NotificationMessage.AppendLine($"{message}");
			await SendMessage();

		}
		protected async Task UpdateNotification(string newLine)
		{
			string[] lines = this.NotificationMessage.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
			string lastLine = lines.LastOrDefault(line => !string.IsNullOrEmpty(line));

			this.NotificationMessage.Replace(lastLine, newLine);

			await SendMessage();
		}
		protected async Task UpdateNotification(string oldLine, string newLine)
		{
			this.NotificationMessage.Replace(oldLine, newLine);
			this.CurrentTelegramMessage.Text = this.NotificationMessage.ToString();
			await SendMessage();
		}
		protected async Task RaiseErrorNotification(string errorMessage, string errorTitle = "Import Failed!")
		{
			var message = $"🛑 *{errorTitle}*\n\n";
			message += $"🏡Description: `{errorMessage}`";

			await SendNotification(message);
		}
		private async Task SendMessage()
		{
			if (this.signalRMessageModel != null)
			{
				this.signalRMessageModel.Message = this.NotificationMessage.ToString();
				await this.notificationService.SendSignalRMessage(this.signalRMessageModel);
			}
			else
			{
				this.CurrentTelegramMessage.Text = this.NotificationMessage.ToString();
				this.CurrentTelegramMessage.MessageId = await this.notificationService.SendTelegramMessage(this.CurrentTelegramMessage);
			}
		}
		private int _userSessionId { get; set; }
        public int UserId { 
            get { return this._userSessionId; } 
            set {
                this._userSessionId = value;
				if (ClientDbContext != null)
				{
					ClientDbContext.UserIdSession = this._userSessionId;
				}
			} 
        }
	}
}
