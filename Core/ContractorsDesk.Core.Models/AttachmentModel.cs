using ContractorsDesk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class AttachmentModel
	{
		public string FileName { get; set; }
		public string FileUrl { get; set; }
		public Stream FileStream { get; set; }
		public string Directory { get; set; }
	}
}
