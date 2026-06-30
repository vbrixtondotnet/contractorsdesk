using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Email
{
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    public int? SenderId { get; set; }

    public string MessageId { get; set; } = null!;

    public string? ReplyToMessageId { get; set; }

    public string Subject { get; set; } = null!;

    public string? Body { get; set; }

    public string? Cc { get; set; }

    public string? Bcc { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public bool IsRead { get; set; }

    public string? From { get; set; }

    public string? To { get; set; }

    public int? EmailType { get; set; }

    public string? PostMarkReferences { get; set; }

    public bool IsArchived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<EmailAttachment> EmailAttachments { get; set; } = new List<EmailAttachment>();
}
