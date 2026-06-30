using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class UserBookmark
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Url { get; set; } = null!;

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }
}
