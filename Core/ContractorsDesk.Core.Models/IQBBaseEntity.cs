using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public interface IQBBaseEntity
	{
		string Id { get; set; }
		QBMetaDataModel? MetaData { get; set; }
	}
}
