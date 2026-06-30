using ContractorsDesk.Core.Dto.@base;

namespace ContractorsDesk.Core.Dto
{
	public class PermissionDto : BaseDto
	{
        public int? PermissionType { get; set; }
        public string? Description { get; set; }
    }
}
