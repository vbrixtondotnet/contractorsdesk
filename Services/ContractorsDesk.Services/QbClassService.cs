using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services
{
	public class QbClassService : BaseService, IQbClassService
	{
		public QbClassService(
			ClientDbContext clientDataDbContext, 
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDataDbContext, masterDbContext, configuration) { }
		public async Task<List<QbClassDto>> GetActiveQbClassesAsync()
		{
			var qbClasses = await ClientDbContext.Qbclasses
				.Where(a =>
					a.IsArchived == false && 
					a.IsDeleted == false && 
					(a.ActiveJobs == true || a.ActiveSpecJobs == true))
				.OrderBy(a => a.Name)
				.ToListAsync();

			return mapper.Map<List<QbClassDto>>(qbClasses);
		}
	}
}
