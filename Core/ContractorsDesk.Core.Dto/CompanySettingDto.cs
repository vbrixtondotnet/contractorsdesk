using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class CompanySettingDto
	{
		public int Id { get; set; }

		public string CompanyName { get; set; } = null!;

		public string? CompanyEmail { get; set; }

		public string? GeneralContractorName { get; set; }

		private string? companyLogoUrl;
		public string? CompanyLogoUrl
		{
			get
			{
				return string.IsNullOrEmpty(companyLogoUrl) ? "https://contractorsdeskstorage.blob.core.windows.net/companyfiles/defaults/company-logo/default.png" : companyLogoUrl;
			}
			set { this.companyLogoUrl = value; }
		}

		public int CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime? DateUpdated { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? PhoneNumber { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

		public bool QuickbooksConnected { get; set; }
    }
}
