using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.Services
{
    public class ProposalTemplatesService : BaseService, IProposalTemplatesService
	{
		private readonly ICacheService cacheService;
		public ProposalTemplatesService(
			ICacheService cacheService,
			ClientDbContext clientDbContext,
			IMapper mapper )
			: base(mapper, clientDbContext) 
		{
			this.cacheService = cacheService;
		}

		#region Public Methods
		public async Task<ProposalTemplateDto> GetByIdAsync(Guid id)
		{
			var dbProposalTemplate = id == Guid.Empty ?
				await ClientDbContext.ProposalTemplates.FirstOrDefaultAsync(pt => pt.IsDefault) :
				await ClientDbContext.ProposalTemplates.FirstOrDefaultAsync(pt => pt.Id == id);

			var proposalTemplate = new ProposalTemplateDto
			{
				Id = dbProposalTemplate.Id,
				Name = dbProposalTemplate.Name,
				IsDefault = dbProposalTemplate.IsDefault
			};

			proposalTemplate.Categories = await GetLineItemsAsync(proposalTemplate.Id);
			return proposalTemplate;
		}
		public async Task<List<ProposalTemplateLineItemDto>> GetLineItemsAsync(Guid id)
		{
			var proposalTemplateId = id;

			var dbParents = await ClientDbContext.ProposalTemplatesLineItems
				.Where(p => p.ParentId == null && p.ProposalTemplateId == proposalTemplateId && !p.IsDeleted)
				.Include(x => x.InverseParent)
				.OrderBy(p => p.Sequence)
				.ToListAsync();

			dbParents.ForEach(parent =>
			{
				var index = 1;

				parent.InverseParent = parent.InverseParent
					.Where(li => !li.IsDeleted)
					.OrderBy(child => child.Sequence)
					.ToList();

				foreach (var item in parent.InverseParent)
				{
					item.Sequence = index;
					index++;
				}
			});

			return mapper.Map<List<ProposalTemplateLineItemDto>>(dbParents);
		}
		public async Task<List<ProposalTemplateDto>> GetProposalTemplatesAsync()
		{
			var dbProposalTemplates = await ClientDbContext.ProposalTemplates
					.Where(x => x.IsActive)
					.OrderByDescending(x => x.IsDefault)
					.ThenBy(x => x.Name)
					.ToListAsync();

			var idList = dbProposalTemplates.Select(p => p.Id).ToList();

			var dbProposalTemplateDefaults = await ClientDbContext.ProposalTemplateUserDefaults
				.Where(i => idList.Contains(i.TemplateId))
				.Select(i => i.TemplateId)
				.ToListAsync();

			var userIds = dbProposalTemplates.Select(pt => pt.CreatedBy).Distinct().ToList();
			var owners = await ClientDbContext.Users
				.Where(user => userIds.Contains(user.Id))
				.ToListAsync();

			var proposalTemplatesDto = dbProposalTemplates.Select(pt =>
			{
				var templateOwners = owners
				   .Where(user => user.Id == pt.CreatedBy)
					   .Select(user => new ApplicationUserShortDetailsDto
					   {
						   Id = user.Id,
						   FirstName = user.FirstName,
						   LastName = user.LastName,
					   }).ToList();
				return new ProposalTemplateDto
				{
					Id = pt.Id,
					Name = pt.Name,
					IsDefault = pt.IsDefault,
					Owner = templateOwners,
					CanBeDeleted = dbProposalTemplateDefaults.FirstOrDefault(p => p == pt.Id) == Guid.Empty,
					DateCreated = pt.DateCreated,
					
				};
			}).ToList();

			return proposalTemplatesDto;
		}
		public async Task<ProposalTemplateDto> CreateProposalTemplateAsync(ProposalTemplatePayload payload)
		{
			var dbProposalTemplate = new ProposalTemplate
			{
				Id = Guid.NewGuid(),
				Name = payload.Name,
				IsDefault = payload.Default,
				ProposalTemplatesLineItems = new List<ProposalTemplatesLineItem>(),
				IsActive = true
			};

			ClientDbContext.ProposalTemplates.Add(dbProposalTemplate);
			var existingCategories = await ClientDbContext.EstimateCategories
				.ToListAsync();

			var newCategories = new List<EstimateCategory>();

			foreach (var category in payload.LineItems)
			{
				// Check if a matching EstimateCategory exists
				var estimateCategory = existingCategories.FirstOrDefault(ec => ec.Name.ToLower() == category.Name.ToLower());

				if (estimateCategory == null)
				{
					// Create a new category if it doesn't exist
					estimateCategory = new EstimateCategory
					{
						Id = Guid.NewGuid(),
						Name = category.Name,
						Sequence = category.Sequence
					};

					existingCategories.Add(estimateCategory);
					newCategories.Add(estimateCategory);
				}

				var dbCategory = new ProposalTemplatesLineItem
				{
					Id = Guid.NewGuid(),
					Name = category.Name,
					Sequence = category.Sequence,
					ProposalTemplateId = dbProposalTemplate.Id,
					EstimateCategoryId = estimateCategory.Id,
					InverseParent = category.LineItems
						.Select(lineItem =>
						{
							var existingLineItem = existingCategories
								.FirstOrDefault(li => li.Name.ToLower() == lineItem.Name.ToLower() && li.ParentEstimateCategoryId == estimateCategory.Id);

							if (existingLineItem == null)
							{
								var newCategory = new EstimateCategory
								{
									Id = Guid.NewGuid(),
									Name = lineItem.Name,
									Sequence = lineItem.Sequence,
									ParentEstimateCategoryId = estimateCategory.Id
								};
								existingCategories.Add(newCategory);
								newCategories.Add(newCategory);

								lineItem.EstimateCategoryId = newCategory.Id;
							}

							return CreateNewTemplateLineItem(lineItem, dbProposalTemplate.Id);
						})
						.ToList()
				};

				dbProposalTemplate.ProposalTemplatesLineItems.Add(dbCategory);
			}

			if (newCategories.Any())
			{
				ClientDbContext.EstimateCategories.AddRange(newCategories);
			}

			await SetIfDefault(dbProposalTemplate.Id, payload);
			await ClientDbContext.SaveChangesAsync();
			return mapper.Map<ProposalTemplateDto>(dbProposalTemplate);
		}
		public async Task<ProposalTemplateDto> UpdateProposalTemplateAsync(ProposalTemplatePayload payload)
		{
			var dbProposalTemplate = await ClientDbContext.ProposalTemplates.FirstOrDefaultAsync(p => p.Id == payload.Id)
				?? throw new ArgumentException("Proposal Template not found.");

			dbProposalTemplate.Name = payload.Name;

			var proposalTemplateLineItems = await ClientDbContext.ProposalTemplatesLineItems
				.Where(p => p.ProposalTemplateId == payload.Id)
				.ToListAsync();

			ClientDbContext.ProposalTemplatesLineItems.RemoveRange(proposalTemplateLineItems);

			await ClientDbContext.SaveChangesAsync();

			var existingCategories = await ClientDbContext.EstimateCategories
				.ToListAsync();

			var newCategories = new List<EstimateCategory>();

			foreach (var category in payload.LineItems)
			{
				// Check if a matching EstimateCategory exists
				var estimateCategory = existingCategories.FirstOrDefault(ec => ec.Name.ToLower() == category.Name.ToLower());

				if (estimateCategory == null)
				{
					// Create a new category if it doesn't exist
					estimateCategory = new EstimateCategory
					{
						Id = Guid.NewGuid(),
						Name = category.Name,
						Sequence = category.Sequence
					};

					existingCategories.Add(estimateCategory);
					newCategories.Add(estimateCategory);
				}

				var dbCategory = new ProposalTemplatesLineItem
				{
					Id = Guid.NewGuid(),
					Name = category.Name,
					Sequence = category.Sequence,
					ProposalTemplateId = dbProposalTemplate.Id,
					EstimateCategoryId = estimateCategory.Id,
					InverseParent = category.LineItems
						.Select(lineItem =>
						{
							var existingLineItem = existingCategories
								.FirstOrDefault(li => li.Name.ToLower() == lineItem.Name.ToLower() && li.ParentEstimateCategoryId == estimateCategory.Id);

							if (existingLineItem == null)
							{
								var newCategory = new EstimateCategory
								{
									Id = Guid.NewGuid(),
									Name = lineItem.Name,
									Sequence = lineItem.Sequence,
									ParentEstimateCategoryId = estimateCategory.Id
								};
								existingCategories.Add(newCategory);
								newCategories.Add(newCategory);

								lineItem.EstimateCategoryId = newCategory.Id;
							}

							return CreateNewTemplateLineItem(lineItem, dbProposalTemplate.Id);
						})
						.ToList()
				};

				dbProposalTemplate.ProposalTemplatesLineItems.Add(dbCategory);
			}

			if (newCategories.Any())
			{
				ClientDbContext.EstimateCategories.AddRange(newCategories);
			}

			await SetIfDefault(dbProposalTemplate.Id, payload);
			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<ProposalTemplateDto>(dbProposalTemplate);
		}
		public async Task<ProposalTemplateUserDefaultDto?> GetProposalTemplateUserDefault()
		{
			var proposalTemplate = await ClientDbContext.ProposalTemplateUserDefaults
				.FirstOrDefaultAsync(p => p.UserId == this.UserId);

			if (proposalTemplate != null)
			{
				return new ProposalTemplateUserDefaultDto
				{
					TemplateId = proposalTemplate.TemplateId,
					UserId = this.UserId
				};
			}

			return null;
		}
		public async Task SaveProposalTemplateUserDefault(Guid id)
		{
			var proposalTemplateUserToDelete = await ClientDbContext.ProposalTemplateUserDefaults
				.FirstOrDefaultAsync(p => p.UserId == this.UserId);

			if (proposalTemplateUserToDelete != null)
				ClientDbContext.ProposalTemplateUserDefaults.Remove(proposalTemplateUserToDelete);
			await ClientDbContext.SaveChangesAsync();

			var proposalTemplateUser = new ProposalTemplateUserDefault
			{
				TemplateId = id,
				UserId = this.UserId
			};

			ClientDbContext.ProposalTemplateUserDefaults.Add(proposalTemplateUser);
			await ClientDbContext.SaveChangesAsync();
		}
		public async Task<bool> RemoveProposalTemplate(Guid id)
		{
			//var dbProposalTemplateUser = await clientDbContext.ProposalTemplateUserDefaults
			//	.FirstOrDefaultAsync(p => p.TemplateId == id);

			//if (dbProposalTemplateUser != null)
			//	throw new ArgumentException("This proposal template is currently set as default by another user.");

			var dbProposalTemplate = await ClientDbContext.ProposalTemplates.FirstOrDefaultAsync(p => p.Id == id);
			if (dbProposalTemplate == null) throw new ArgumentException("Proposal Template not found.");

			var relatedLineItems = ClientDbContext.ProposalTemplatesLineItems
				.Where(li => li.ProposalTemplateId == id);
			ClientDbContext.ProposalTemplatesLineItems.RemoveRange(relatedLineItems);

			var proposals = ClientDbContext.Proposals
				.Where(p => p.TemplateId == id).ToList();

			proposals.ForEach(p => p.TemplateId = null);

			ClientDbContext.ProposalTemplates.Remove(dbProposalTemplate);
			await ClientDbContext.SaveChangesAsync();

			return true;
		}
		public async Task<bool> DeleteProposalTemplate(Guid id) 
		{

            var template = await ClientDbContext.ProposalTemplates
				.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (template == null)
            {
                return false;
            }

            template.IsActive = false;
            await ClientDbContext.SaveChangesAsync();

            return true;
		}
        #endregion

        #region Private Methods

        private async Task SetIfDefault(Guid templateId, ProposalTemplatePayload payload)
		{
			var proposalTemplateUserDefault = await ClientDbContext.ProposalTemplateUserDefaults
					.FirstOrDefaultAsync(p => p.UserId == this.UserId);

			if (payload.Default)
			{
				if (proposalTemplateUserDefault != null)
					ClientDbContext.ProposalTemplateUserDefaults.Remove(proposalTemplateUserDefault);

				await ClientDbContext.SaveChangesAsync();

				var proposalTemplateUser = new ProposalTemplateUserDefault
				{
					TemplateId = templateId,
					UserId = this.UserId
				};

				ClientDbContext.ProposalTemplateUserDefaults.Add(proposalTemplateUser);
			}

			if(proposalTemplateUserDefault.TemplateId == templateId)
			{
				await this.cacheService.RemoveAsync("NewProposal");
			}
		}
		private ProposalTemplatesLineItem CreateNewTemplateLineItem(ProposalTemplateItemPayload lineItem, Guid proposalTemplateId)
		{
			var dbLineItem = mapper.Map<ProposalTemplatesLineItem>(lineItem);
			dbLineItem.Id = Guid.NewGuid();
			dbLineItem.EstimateCategoryId = lineItem.EstimateCategoryId;
			dbLineItem.ProposalTemplateId = proposalTemplateId;

			return dbLineItem;
		}

		private void CreateEstimateCategory(ProposalTemplateCategoryPayload category, List<EstimateCategory> existingEstimateCategories, List<EstimateCategory> newEstimateCategories)
		{
			var estimateCategory = existingEstimateCategories.FirstOrDefault(ec => ec.Name == category.Name);
			if (estimateCategory == null)
			{
				estimateCategory = new EstimateCategory
				{
					Id = Guid.NewGuid(),
					Name = category.Name,
					Sequence = category.Sequence
				};

				existingEstimateCategories.Add(estimateCategory);
				newEstimateCategories.Add(estimateCategory);
			}

			foreach (var lineItem in category.LineItems)
			{
				var existingLineItem = existingEstimateCategories
								.FirstOrDefault(li => li.Name == lineItem.Name && li.ParentEstimateCategoryId == estimateCategory.Id);
				if (existingLineItem == null)
				{
					var newCategory = new EstimateCategory
					{
						Id = Guid.NewGuid(),
						Name = lineItem.Name,
						Sequence = lineItem.Sequence,
						ParentEstimateCategoryId = estimateCategory.Id
					};

					existingEstimateCategories.Add(newCategory);
					newEstimateCategories.Add(newCategory);
				}
			}

		}
		private ProposalTemplatesLineItem CreateCategoryAndLineItems(ProposalTemplateCategoryPayload category, Guid proposalTemplateId)
		{
			var dbCategory = new ProposalTemplatesLineItem
			{
				Id = Guid.NewGuid(),
				Name = category.Name,
				Sequence = category.Sequence,
				ProposalTemplateId = proposalTemplateId,
			};

			foreach (var lineItem in category.LineItems)
			{
				var dbLineItem = CreateNewTemplateLineItem(lineItem, proposalTemplateId);
				dbCategory.InverseParent.Add(dbLineItem);
			}

			return dbCategory;
		}
		private void HandleDeletedTemplateLineItems(ProposalTemplatesLineItem existingCategory, ProposalTemplateCategoryPayload category)
		{
			//handle delete items
			var deletedLineItems = existingCategory.InverseParent.Where(ci => !category.LineItems.Any(ni => ni.Id == ci.Id)).ToList();
			deletedLineItems.ForEach(lineItem =>
			{
				ClientDbContext.ProposalTemplatesLineItems.Remove(lineItem);
			});
		}
		private void UpdateTemplateLineItems(ProposalTemplateCategoryPayload category, List<ProposalTemplatesLineItem> existingCategories, Guid proposalTemplateId)
		{
			foreach (var lineItem in category.LineItems)
			{
				var dbCategory = existingCategories.FirstOrDefault(c => c.Id == lineItem.ParentId)
					?? throw new ArgumentException("Template Category not found.");

				var dbLineItem = dbCategory.InverseParent.FirstOrDefault(l => l.Id == lineItem.Id);

				if (dbLineItem == null)
				{
					dbLineItem = CreateNewTemplateLineItem(lineItem, proposalTemplateId);
					dbCategory.InverseParent.Add(dbLineItem);
				}
				else
				{
					dbLineItem.Name = lineItem.Name;
					dbLineItem.Description = lineItem.Description;
					dbLineItem.Amount = lineItem.Amount;
					dbLineItem.Sequence = lineItem.Sequence;
					dbLineItem.Percentage = lineItem.Percentage;
					dbLineItem.EstimateCategoryId = lineItem.EstimateCategoryId;
				}
			}
		}

		#endregion
	}
}
