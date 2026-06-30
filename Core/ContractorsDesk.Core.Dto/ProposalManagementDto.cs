using ContractorsDesk.Core.Dto.@base;

namespace ContractorsDesk.Core.Dto
{
	public class ProposalManagementDto : AuditableDto
	{
		public Guid Id { get; set; }
		public int Number { get; set; }
		public string DocStatus { get; set; }
		public string Date { get; set; }
		public decimal TotalAmount { get; set; }

		private string? _client;
        private string? _clientEmailAddress;
        private string? _project;
		private string? _template;
        public string Client
        {
            get { return _client ?? string.Empty; }
            set { _client = value; }
        }
        public string ClientEmailAddress
        {
            get { return _clientEmailAddress ?? string.Empty; }
            set { _clientEmailAddress = value; }
        }
        public Guid ProjectId { get; set; }
		public Guid? QbClassId { get; set; }
		public string Project
		{
			get { return _project ?? string.Empty; }
			set { _project = value; }
		}
		public string Template
		{
			get { return _template ?? string.Empty; }
			set { _template = value; }
		}

		public int DocStatusId
		{
			get
			{
				return DocStatus.ToLower() == "draft" ? 1 : 2;
			}
		}
		public List<ApplicationUserShortDetailsDto> Supervisors { get; set; } = new List<ApplicationUserShortDetailsDto>();

        public bool IsArchived { get; set; }

		public ApplicationUserShortDetailsDto CreatedBy { get; set; }

		public ApplicationUserShortDetailsDto? UpdatedBy { get; set; }
	}
}
