using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class PostMarkInboundEmailModel
	{
		public string From { get; set; }
		public string FromName { get; set; }
		public EmailIdentity FromFull { get; set; }
		public string To { get; set; }
		public List<EmailIdentity> ToFull { get; set; }
		public string Cc { get; set; }
		public List<EmailIdentity> CcFull { get; set; }
		public string Bcc { get; set; }
		public List<EmailIdentity> BccFull { get; set; }
		public string OriginalRecipient { get; set; }
		public string Subject { get; set; }
		public string MessageID { get; set; }
		public string ReplyTo { get; set; }
		public string MailboxHash { get; set; }
		public string Date { get; set; }
		public string TextBody { get; set; }
		public string HtmlBody { get; set; }
		public string StrippedTextReply { get; set; }
		public string StrippedSignature { get; set; }
		public string Tag { get; set; }
		public List<Header> Headers { get; set; }
		public List<Attachment> Attachments { get; set; }
		public string SpamAssassinScore { get; set; }
		public string SpamAssassinStatus { get; set; }
		public string SpamAssassinReport { get; set; }
		public string SenderIP { get; set; }
	}

	public class EmailIdentity
	{
		public string Email { get; set; }
		public string Name { get; set; }
		public string MailboxHash { get; set; }
	}

	public class Header
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}

	public class Attachment
	{
		public string Name { get; set; }
		public string Content { get; set; } // Base64-encoded
		public string ContentType { get; set; }
		public int ContentLength { get; set; }
	}
}
