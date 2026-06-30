namespace ContractorsDesk.Core.Dto
{
	public class RoleDto
	{
		public int Id { get; set; }
		public int CategoryId { get; set; }
		public string Name {  get; set; }
		public List<PermissionDto> Permissions { get; set; }
	}
}
