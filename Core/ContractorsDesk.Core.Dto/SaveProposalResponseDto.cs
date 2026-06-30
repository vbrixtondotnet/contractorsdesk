using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
    public class SaveProposalResponseDto
    {
        public List<ProjectMatchResultDto>? MatchingProjects { get; set; }
		public List<ProjectMatchResultDto>? MatchingDraftProposals { get; set; }
		public ProposalDto? Proposal { get; set; }
    }
}
