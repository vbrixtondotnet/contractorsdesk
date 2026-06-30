using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class EstimateDataMappingService : BaseService, IEstimateDataMappingService
	{
		public EstimateDataMappingService(
			ClientDbContext clientDataDbContext, 
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDataDbContext, masterDbContext, configuration) { }

		public async Task<List<EstimateCategoryShortDetailsDto>> GetEstimateCategoriesAsync()
		{
			return await ClientDbContext.EstimateCategories
				.Where(e => e.ParentEstimateCategoryId != null)
				.Join(
					ClientDbContext.EstimateCategories,    // Inner join with the same table
					e => e.ParentEstimateCategoryId,          // Foreign key in the EstimateCategories table
					ep => ep.Id,                              // Primary key in the EstimateCategories table
					(e, ep) => new EstimateCategoryShortDetailsDto                           // Projection
					{
						Id = e.Id,
						Name = e.Name,
						ParentEstimateCategoryId = e.ParentEstimateCategoryId,
						Parent = ep.Name,
						ParentSequence = ep.Sequence
					}
				)
				.OrderBy(x => x.ParentSequence)
				.ToListAsync();
		}
		public async Task<List<EstimateCategoryShortDetailsDto>> GetParentEstimateCategoriesAsync()
		{
			return await ClientDbContext.EstimateCategories
				.Where(e => e.ParentEstimateCategoryId == null)
				.Select(e => new EstimateCategoryShortDetailsDto
				{
					Id = e.Id,
					Name = e.Name,
					ParentEstimateCategoryId = null,
					Parent = string.Empty,
					ParentSequence = e.Sequence
				})
				.OrderBy(e => e.ParentSequence)
				.ToListAsync();
		}
		public async Task<List<EstimateDataMappingDto>> GetEstimateDataMappingsAsync()
		{
			return await ClientDbContext.VwEstimateDataMappings
							.OrderBy(e => string.IsNullOrEmpty(e.EstimateCategory) ? 1 : 0) // Place empty values at the end
							.ThenBy(e => e.FullyQualifiedName)
							.Select(x => new EstimateDataMappingDto
							{
								MappingId = x.MappingId,
								EstimateCategoryId = x.EstimateCategoryId,
								EstimateCategory = x.EstimateCategory,
								FullyQualifiedName = x.FullyQualifiedName,
								ParentCategory = x.ParentCategory,
								AccountId = x.AccountId
							})
							.ToListAsync();

		}
		public async Task<List<EstimateDataMappingDto>> SaveEstimateDataMappingsAsync(List<EstimateDataMappingDto> estimateDataMappings)
		{
			// process updated
			var updatedMappings = estimateDataMappings.Where(e => e.Updated).ToList();
			foreach (var updatedMapping in updatedMappings)
			{
				var dataMapping = ClientDbContext.EstimateMappings.FirstOrDefault(m => m.Id == updatedMapping.MappingId);
				if (dataMapping != null)
				{
					dataMapping.EstimateSubCategoryId = updatedMapping.EstimateCategoryId;
					ClientDbContext.EstimateMappings.Entry(dataMapping).State = EntityState.Modified;
				}
			}

			// process added
			var newMappings = estimateDataMappings.Where(e => e.Added).ToList();
			foreach (var newMapping in newMappings)
			{
				var dataMapping = new EstimateMapping
				{
					Id = Guid.NewGuid(),
					QbaccountId = newMapping.AccountId,
					EstimateSubCategoryId = newMapping.EstimateCategoryId,
					AccountType = "Expenses",
					Created = DateTime.UtcNow
				};

				await ClientDbContext.EstimateMappings.AddAsync(dataMapping);
			}

			await ClientDbContext.SaveChangesAsync();
			return await this.GetEstimateDataMappingsAsync();
		}
		public async Task<List<EstimateCategoryShortDetailsDto>> SaveEstimateCategoriesAsync(List<EstimateCategoryShortDetailsDto> estimateCategories)
		{
			var dbEstimateCategories = await ClientDbContext.EstimateCategories
				.Where(ec=> ec.ParentEstimateCategoryId != null)
				.ToListAsync();

			estimateCategories.ForEach(ec =>
			{
				var dbEstimateCategory = dbEstimateCategories.FirstOrDefault(e => e.Id == ec.Id);
				if (dbEstimateCategory != null)
				{
					dbEstimateCategory.Name = ec.Name;
					dbEstimateCategory.ParentEstimateCategoryId = ec.ParentEstimateCategoryId;
				}
			});

			await ClientDbContext.SaveChangesAsync();
			return await this.GetEstimateCategoriesAsync();
		}
	}
}
