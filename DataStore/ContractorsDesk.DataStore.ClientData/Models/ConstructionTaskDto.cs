namespace ContractorsDesk.DataStore.Client.Models;

public partial class ConstructionTaskDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Sequence { get; set; }

    public int? Duration { get; set; }

    public Guid? ParentTaskId { get; set; }

    public Guid? Pred1Id { get; set; }

    public int? Pred1Lag { get; set; }

    public Guid? Pred2Id { get; set; }

    public int? Pred2Lag { get; set; }

    public Guid? Pred3Id { get; set; }

    public int? Pred3Lag { get; set; }

    public Guid? ProjectManagementId { get; set; }

    public Guid? ProjectManagementLineId { get; set; }

    public virtual ProjectManagementLine? ProjectManagementLine { get; set; }
}
