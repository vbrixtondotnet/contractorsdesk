using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public  class UploadClientDocumentResult
	{
		public string BaseFileName { get; set; } = null!;
		public string FileName { get; set; } = null!;
		public string Url { get; set; } = null!;
		public int Version { get; set; }
		public Stream FileStream { get; set; } = null!;
	}
}
