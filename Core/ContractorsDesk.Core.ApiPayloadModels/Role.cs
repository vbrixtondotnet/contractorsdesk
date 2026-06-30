
using ContractorsDesk.Core.ApiPayloadModels.@base;
namespace ContractorsDesk.Core.ApiPayloadModels
{
    public class Role : BaseModel
	{
		public string Name { get; set; } 
		public List<Permission> Permissions { get; set; }
	}
}
