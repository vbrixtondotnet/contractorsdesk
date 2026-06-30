using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class SubContractorPayloadModel
	{
		public Guid? Id { get; set; }
		public Guid? ProjectId { get; set; }
		public bool IsNew { get; set; } = false;
		public string? Category { get; set; }
		public string Name { get; set; } = null!;

		public string? Address { get; set; } = string.Empty;

		public string? City { get; set; } = string.Empty;

		public string? State { get; set; } = string.Empty;

        public string? Company { get; set; } = string.Empty;
		public string Email { get; set; } = null!;

		public string? Phone { get; set; } = string.Empty;

		public string? LicenseNo { get; set; } = string.Empty;

		public DateOnly? LicenseExp { get; set; }
		public string? LiabilityInsurancePolicyNo { get; set; }

		public DateOnly? LiabilityInsuranceExpiry { get; set; }

		public string? CompInsurancePolicyNo { get; set; }

		public DateOnly? CompInsuranceExpiry { get; set; }

		public string? BondInsurancePolicyNo { get; set; }

		public DateOnly? BondInsuranceExpiry { get; set; }

		public int CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime? DateUpdated { get; set; }

		public bool IsActive { get; set; } = true;

        public string? Zip { get; set; } = string.Empty;
    }
}
