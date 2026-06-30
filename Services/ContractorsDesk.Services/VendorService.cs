using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.Services
{
	public class VendorService : BaseService, IVendorService
	{
		public VendorService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext) 
		{

		}

		public async Task<List<VendorDto>> GetVendorsAsync(string? searchKey = null)
		{
			if(!string.IsNullOrEmpty(searchKey) && searchKey.Length > 2)
			{
				//return await GetAllSubcontractorsByKeywordAsync(searchKey);
			}

            var vendors = await ClientDbContext.Vendors
                 .AsNoTracking()
                 .Where(x => x.IsActive == true)
                 .OrderBy(x => x.Name)
                 .ToListAsync();

            return mapper.Map<List<VendorDto>>(vendors);
		}

        public async Task<VendorDto> AddVendorAsync(VendorPayload model)
        {
            Vendor? dbVendor = null;

            if (!string.IsNullOrWhiteSpace(model.Category))
            {
                var existingCategory = await ClientDbContext.VendorCategories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == model.Category.ToLower());

                if (existingCategory == null)
                {
                    var newCategory = new VendorCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Category,
                        IsActive = true,
                    };

                    ClientDbContext.VendorCategories.Add(newCategory);
                }
            }

            if (!model.IsNew)
            {
                dbVendor = await ClientDbContext.Vendors
                    .FirstOrDefaultAsync(s => s.Id == model.Id);

                if (dbVendor != null)
                {
                    mapper.Map(model, dbVendor);
                }
            }
            else
            {
                dbVendor = mapper.Map<Vendor>(model);
                dbVendor.Id = Guid.NewGuid();
                dbVendor.DateCreated = DateTime.UtcNow;
                dbVendor.CreatedBy = this.UserId;
                dbVendor.IsActive = true;

                ClientDbContext.Vendors.Add(dbVendor);
            }

            await ClientDbContext.SaveChangesAsync();

            return mapper.Map<VendorDto>(dbVendor);
        }

        public async Task<bool> DeleteVendorAsync(Guid id)
        {
            var vendor = await ClientDbContext.Vendors
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
                return false;

            if (!vendor.IsActive)
                return true;

            vendor.IsActive = false;

            ClientDbContext.Vendors.Update(vendor);
            await ClientDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetVendorTypesAsync()
        {
            var categories = await ClientDbContext.VendorCategories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            return categories;
        }

        //      private async Task<List<SubContractorDto>> GetAllSubcontractorsByKeywordAsync(string searchKey)
        //{
        //	var subcontractors = await clientDbContext.SubContractors
        //		.AsNoTracking()
        //		 .Where(c => c.Name.Contains(searchKey) ||
        //					c.Email.Contains(searchKey))
        //		.OrderBy(x => x.Name)
        //		.ToListAsync();

        //	return mapper.Map<List<SubContractorDto>>(subcontractors);
        //}
    }
}
