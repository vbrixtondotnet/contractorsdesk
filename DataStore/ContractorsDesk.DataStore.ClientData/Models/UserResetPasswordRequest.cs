using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class UserResetPasswordRequest
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime DateSent { get; set; }

    public string? SentStatus { get; set; }

    public bool? IsUsed { get; set; }

    public string? ResetLink { get; set; }
}
