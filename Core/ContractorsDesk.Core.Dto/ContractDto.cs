namespace ContractorsDesk.Core.Dto
{
	public class ContractDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string BodyTemplate { get; set; }

		public string DefaultFolderName { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedById { get; set; }

		public DateTime DateModified { get; set; }

		public int ModifiedById { get; set; }
	}
}
