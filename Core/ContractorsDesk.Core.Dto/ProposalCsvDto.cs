using ContractorsDesk.Core.Enums;

namespace ContractorsDesk.Core.Dto
{
	public class ProposalCsvDto
    {
        public int? Sequence { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }

    }
}
