namespace ContractorsDesk.WebPortal.Models
{
	public class PermissionViewModel
	{
		public string UserRole { get; set; }
		public bool CanManageOwnJobs{ get; set; }
		public bool CanManageAllJobs{ get; set; }
		public bool CanManageOwnEstimates { get; set; }
		public bool CanManageAllEstimates { get; set; }
		public bool CanManageAllActionItems { get; set; }
		public bool CanManageOwnActionItems { get; set; }
		public bool CanManageCompanyUsers { get; set; }
		public bool CanManageCompanyRoles { get; set; }
		public bool CanManageSystemUsers { get; set; }
		public bool CanManageSystemRoles { get; set; }
		public bool CanAssignJobs { get; set; }
		public bool CanAssignEstimates { get; set; }
		public bool CanAssignActionItems { get; set; }
		public bool CanAccessClientJobs { get; set; }
		public bool CanManageDataMapping { get; set; }
        public bool CanEditAcceptedProposals { get; set; }
        public bool CanEditCompletedSchedules { get; set; }
		public bool CanAccessCompanySettings { get; set; }
        public bool CanAccessDataSyncServices { get; set; }
    }
}

