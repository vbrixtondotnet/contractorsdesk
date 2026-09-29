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
		public async Task<EstimateDataMappingPageDto> GetEstimateDataMappingsAsync()
		{
			var categories = await ClientDbContext.EstimateCategories
				.AsNoTracking()
				.ToListAsync();

			var mappings = await ClientDbContext.EstimateMappings
				.AsNoTracking()
				.Where(m => m.EstimateSubCategoryId != null && m.QbaccountId != null)
				.Select(m => new
				{
					m.Id,
					EstimateSubCategoryId = m.EstimateSubCategoryId!.Value,
					QbaccountId = m.QbaccountId!.Value,
					FullyQualifiedName = m.Qbaccount != null
						? (m.Qbaccount.FullyQualifiedName ?? m.Qbaccount.Name)
						: null
				})
				.ToListAsync();

			var mappingsByCategory = mappings
				.GroupBy(m => m.EstimateSubCategoryId)
				.ToDictionary(
					g => g.Key,
					g => g
						.GroupBy(m => m.QbaccountId)
						.Select(accountGroup => accountGroup.First())
						.OrderBy(m => m.FullyQualifiedName)
						.Select(m => new EstimateAccountMappingDto
						{
							MappingId = m.Id,
							AccountId = m.QbaccountId,
							EstimateCategoryId = m.EstimateSubCategoryId,
							FullyQualifiedName = m.FullyQualifiedName ?? string.Empty
						})
						.ToList());

			var allParents = categories
				.Where(c => c.ParentEstimateCategoryId == null)
				.ToList();

			var validParentIds = allParents
				.Where(c => !string.IsNullOrEmpty(c.Name))
				.Select(c => c.Id)
				.ToHashSet();

			var parents = allParents
				.Where(c => validParentIds.Contains(c.Id))
				.OrderBy(c => c.Sequence)
				.ThenBy(c => c.Name)
				.ToList();

			var childrenByParent = categories
				.Where(c =>
					c.ParentEstimateCategoryId != null
					&& validParentIds.Contains(c.ParentEstimateCategoryId.Value))
				.GroupBy(c => c.ParentEstimateCategoryId!.Value)
				.ToDictionary(
					g => g.Key,
					g => g.OrderBy(c => c.Sequence).ThenBy(c => c.Name).ToList());

			var parentIds = allParents.Select(p => p.Id).ToHashSet();
			var groups = parents.Select(parent =>
			{
				childrenByParent.TryGetValue(parent.Id, out var children);
				return new EstimateDataMappingGroupDto
				{
					Id = parent.Id,
					Name = parent.Name,
					Sequence = parent.Sequence,
					Items = (children ?? new List<EstimateCategory>())
						.Select(child => ToMappingItem(child, mappingsByCategory))
						.ToList()
				};
			})
			.Where(group => group.Items.Count > 0)
			.ToList();

			var orphans = categories
				.Where(c => c.ParentEstimateCategoryId != null && !parentIds.Contains(c.ParentEstimateCategoryId.Value))
				.OrderBy(c => c.Sequence)
				.ThenBy(c => c.Name)
				.ToList();

			if (orphans.Count > 0)
			{
				groups.Add(new EstimateDataMappingGroupDto
				{
					Id = Guid.Empty,
					Name = "Uncategorized",
					Sequence = int.MaxValue,
					Items = orphans.Select(child => ToMappingItem(child, mappingsByCategory)).ToList()
				});
			}

			var accounts = (await ClientDbContext.Qbaccounts
				.AsNoTracking()
				.Where(a => a.AccountType == "Expense" && a.FullyQualifiedName != null && a.FullyQualifiedName != "")
				.OrderBy(a => a.FullyQualifiedName)
				.Select(a => new EstimateMappingAccountOptionDto
				{
					Id = a.Id,
					FullyQualifiedName = a.FullyQualifiedName!
				})
				.ToListAsync())
				.Where(a => !string.IsNullOrWhiteSpace(a.FullyQualifiedName))
				.ToList();

			return new EstimateDataMappingPageDto
			{
				Groups = groups,
				Accounts = accounts
			};
		}

		public async Task<EstimateDataMappingPageDto> SaveEstimateDataMappingsAsync(List<EstimateAccountMappingDto> estimateDataMappings)
		{
			var changes = estimateDataMappings ?? new List<EstimateAccountMappingDto>();
			var mappings = await ClientDbContext.EstimateMappings.ToListAsync();

			foreach (var change in changes.Where(c => c.Removed))
			{
				var rows = mappings.Where(m =>
					ClientDbContext.Entry(m).State != EntityState.Deleted
					&& (
						(change.MappingId.HasValue && m.Id == change.MappingId.Value)
						|| (m.QbaccountId == change.AccountId && m.EstimateSubCategoryId == change.EstimateCategoryId)))
					.ToList();

				foreach (var mapping in rows)
				{
					ClientDbContext.EstimateMappings.Remove(mapping);
				}
			}

			foreach (var change in changes.Where(c => c.Added && c.AccountId != Guid.Empty && c.EstimateCategoryId != Guid.Empty))
			{
				var matches = mappings
					.Where(m =>
						m.QbaccountId == change.AccountId
						&& ClientDbContext.Entry(m).State != EntityState.Deleted)
					.ToList();

				if (matches.Count > 0)
				{
					var existing = matches[0];
					if (existing.EstimateSubCategoryId != change.EstimateCategoryId)
					{
						existing.EstimateSubCategoryId = change.EstimateCategoryId;
						existing.Updated = DateTime.UtcNow;
						existing.UpdatedBy = this.UserId.ToString();
					}

					foreach (var extra in matches.Skip(1))
					{
						ClientDbContext.EstimateMappings.Remove(extra);
					}

					continue;
				}

				var created = new EstimateMapping
				{
					Id = Guid.NewGuid(),
					QbaccountId = change.AccountId,
					EstimateSubCategoryId = change.EstimateCategoryId,
					AccountType = "Expenses",
					Created = DateTime.UtcNow,
					CreatedBy = this.UserId.ToString()
				};

				await ClientDbContext.EstimateMappings.AddAsync(created);
				mappings.Add(created);
			}

			await ClientDbContext.SaveChangesAsync();
			return await this.GetEstimateDataMappingsAsync();
		}

		private static EstimateDataMappingItemDto ToMappingItem(
			EstimateCategory category,
			Dictionary<Guid, List<EstimateAccountMappingDto>> mappingsByCategory)
		{
			mappingsByCategory.TryGetValue(category.Id, out var accounts);
			return new EstimateDataMappingItemDto
			{
				Id = category.Id,
				Name = category.Name,
				Sequence = category.Sequence,
				ParentEstimateCategoryId = category.ParentEstimateCategoryId ?? Guid.Empty,
				Accounts = accounts ?? new List<EstimateAccountMappingDto>()
			};
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
