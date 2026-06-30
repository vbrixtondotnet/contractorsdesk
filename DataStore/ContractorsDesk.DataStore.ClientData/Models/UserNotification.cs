using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class UserNotification
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsGlobal { get; set; }

    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    public bool? IsRead { get; set; }

    public int? CreatedById { get; set; }

    public DateTime DateCreated { get; set; }

    public string? RelatedUrl { get; set; }

    public Guid? EmailId { get; set; }

    public string? NextActionEnum { get; set; }
}
