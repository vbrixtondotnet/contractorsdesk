using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProjectEstimateCategoriesAndScheduleItemsDto
	{
		public List<EstimateCategoryItemDto> EstimateCategories { get; set; }
		public List<ProjectScheduleTaskItemDto> ScheduleTasks { get; set; }

	}

	public class EstimateCategoryItemDto
	{
		public Guid EstimateCategoryID { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal? CurrentAmount { get; set; }
	}

	public class ProjectScheduleTaskItemDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}
