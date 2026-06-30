using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
    public class CompanySettingPayload
    {
        public string CompanyName { get; set; }
        public string? CompanyEmail { get; set; }
        public string? GeneralContractorName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
    }
}
