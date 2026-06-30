using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBClassModel : IQBBaseEntity
	{
		public string Name { get; set; }
		public bool SubClass { get; set; }
        public string FullyQualifiedName { get; set; }
        public string Id { get; set; }
		public QBMetaDataModel? MetaData { get; set; }
	}
}
