using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces.@base
{
    public interface IBaseEmailService : IBaseService
	{
		string SubDomain { get; set; }
		int SenderId { get; set; }
		string SenderName { get; set; }
		Task<bool> SendEmailAsync(EmailPayloadModel emailModel);
		Task<SentEmailDto> GetSentEmailByIdAsync(Guid id);
		Task<bool> MarkSentEmailAsRead(Guid id);
		Task<List<SentEmailDto>> GetSentEmailsAsync();
		Task<bool> SendReplyAsync(EmailPayloadModel emailModel);
	}
}
