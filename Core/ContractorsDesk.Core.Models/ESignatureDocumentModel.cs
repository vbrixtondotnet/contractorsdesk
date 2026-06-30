using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class ESignatureDocumentModel
	{
		public string From { get; set; }
		public string To { get; set; }
		public string FromName { get; set; }
		public string FileName { get; set; }
		public string CompanyName { get; set; }
		public byte[] DocumentBytes { get; set; }
	}
}
