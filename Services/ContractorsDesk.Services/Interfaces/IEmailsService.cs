using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IEmailsService : IBaseService
	{
		Task<Email> GetEmailByMessageId(string messageId);
		Task<List<string>> GetToEmailAddresses();
		Task<Guid> SaveEmailAsync(EmailModel emailModel);
		Task<UserInboxDto> GetUserInboxAsync(Roles role, int supervisorId, bool canManageAllProjects);
		Task<UserInboxDto> GetUserSentItemsAsync(Roles role, int supervisorId, bool canManageAllProjects);
		Task<InboxMessageDto> GetInboxMessageByIdAsync(Guid id, int userId);
		Task MarkAsReadAsync(string messageId);
		Task ArchiveAsync(string messageId);
	}
}
