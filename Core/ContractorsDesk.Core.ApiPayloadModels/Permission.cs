using ContractorsDesk.Core.ApiPayloadModels.@base;
namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class Permission : BaseModel
	{
		public bool Read { get; set; }
		public bool Write { get; set; }
	}
}
