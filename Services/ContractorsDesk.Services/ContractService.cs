using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.Services
{
	public class ContractService : BaseService, IContractService
	{
		public ContractService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext)
		{ }

		public async Task<List<ContractDto>> GetContracts()
		{
			var contracts = await ClientDbContext.Contracts
				.AsNoTracking()
				.ToListAsync();

			var contractsDto = mapper.Map<List<ContractDto>>(contracts);

			foreach (var contract in contractsDto)
			{
				contract.BodyTemplate = await GetHtmlTemplateAsync(contract.DefaultFolderName, contract.BodyTemplate);
			}

			return contractsDto;
		}

		public async Task<ContractDto> SaveContract(ContractPayload model)
		{
			Contract? dbContract = null;

			if (!model.IsNew)
			{
				dbContract = await ClientDbContext.Contracts.FirstOrDefaultAsync(s => s.Id == model.Id);

				if (dbContract == null)
					throw new KeyNotFoundException($"EmailTemplate with Id {model.Id} not found.");

				mapper.Map(model, dbContract);


				dbContract.BodyTemplate = await GetHtmlTemplateAsync(dbContract.DefaultFolderName, dbContract.BodyTemplate);
				dbContract.DateModified = DateTime.UtcNow;
				dbContract.ModifiedById = this.UserId;

				ClientDbContext.Contracts.Update(dbContract);
			}
			else
			{
				dbContract = mapper.Map<Contract>(model);
				dbContract.Id = Guid.NewGuid();
				dbContract.DateCreated = DateTime.UtcNow;
				dbContract.CreatedById = this.UserId;
				dbContract.DateModified = DateTime.UtcNow;
				dbContract.ModifiedById = this.UserId;

				ClientDbContext.Contracts.Add(dbContract);
			}

			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<ContractDto>(dbContract);
		}

		private Task<string> GetHtmlTemplateAsync(string folderName, string bodyContent)
		{
			if (!string.IsNullOrEmpty(bodyContent))
				return Task.FromResult(bodyContent);

			var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "HTMLTemplates", $"{folderName}/htmlpage.html");

			if (!File.Exists(htmlFilePath))
			{
				throw new Exception($"The file {htmlFilePath} was not found.");
			}

			var htmlContent = File.ReadAllText(htmlFilePath);

			return Task.FromResult(htmlContent);
		}
	}
}
