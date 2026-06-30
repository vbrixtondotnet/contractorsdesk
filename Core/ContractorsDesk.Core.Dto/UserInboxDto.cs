using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ContractorsDesk.Core.Dto
{
	public class UserInboxDto
	{
		public List<InboxProjectDto> Projects { get; set; }
		public List<InboxMessageDto> Messages { get; set; }
	}

	public class InboxMessageDto
	{
		public Guid Id { get; set; }
		public Guid? ProjectId { get; set; }
		public string? ProjectName { get; set; }
		public Guid MessageId { get; set; }
		public Guid? ReplyToMessageId { get; set; }

		private string? senderName;
		public string? SenderName { get { return senderName ?? this.From; } set { this.senderName = value; } }
		public string From { get; set; }
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string Message { 
			get
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(this.Body);

				// Get the first <div> element
				var firstDiv = doc.DocumentNode.SelectSingleNode("//div[1]");

				// Extract and flatten the text
				var message = firstDiv != null
					? string.Join(" ", firstDiv
						.Descendants()
						.Where(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText))
						.Select(n => n.InnerText.Trim()))
					: this.Body;

				// Remove all newline characters and trim extra spaces
				message = message.Replace("<p>", "").Replace("</p>", "").Trim();

				// Truncate to 50 characters
				var retval = message.Length > 50 ? message.Substring(0, 50) : message;
				return $"{retval}...";

			}
		}
		public string SenderInitials
		{
			get {
				return string.Concat(this.SenderName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => char.ToUpper(word[0])));
			}
		}
		public DateTime DateCreated { get; set; }
		public bool IsRead { get; set; }
		public List<EmailAttachmentDto> Attachments { get; set; }
	}

	public class InboxProjectDto
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public int TotalCount { get; set; }
		public int UnreadCount { get; set; }
	}
}
