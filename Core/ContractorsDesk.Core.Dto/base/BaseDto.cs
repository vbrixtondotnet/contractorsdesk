namespace ContractorsDesk.Core.Dto.@base
{
	public abstract class BaseDto
    {
        public int Id { get; set; }
    }

    public abstract class AuditableDto
    {
        public ApplicationUserShortDetailsDto CreatedBy { get; set; }
        public ApplicationUserShortDetailsDto? UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
