namespace ContractorsDesk.DataStore.Client.Models;

public partial class SupervisorProject
{
    public int Id { get; set; }

    public int? SupervisorId { get; set; }

    public Guid? ProjectId { get; set; }

    public DateOnly? DateAssigned { get; set; }

    public virtual Qbclass? Project { get; set; }
}
