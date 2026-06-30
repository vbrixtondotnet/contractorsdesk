using System.ComponentModel.DataAnnotations;

namespace ContractorsDesk.Core.Models.@base
{
	public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
