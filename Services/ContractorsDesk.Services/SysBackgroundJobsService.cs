using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
 
namespace ContractorsDesk.Services
{
    public class SysBackgroundJobsService : BaseService, ISysBackgroundJobsService
    {
        public SysBackgroundJobsService(
            ClientDbContext clientDataDbContext,
            MasterDbContext masterDbContext,
            IMapper mapper,
            IConfiguration configuration) :
            base(mapper, clientDataDbContext, masterDbContext, configuration)
        { }

        public async Task<IEnumerable<SysBackgroundJobsDto>> GetAllSysBackgroundJobsAsync()
        {
            var sysBackgroundJobs = await ClientDbContext.SysBackgroundJobs.ToListAsync();
            return mapper.Map<IEnumerable<SysBackgroundJobsDto>>(sysBackgroundJobs);
        }
        public async Task<SysBackgroundJobsDto> GetSysBackgroundJobAsync(int sysJobsId)
        {
            var sysBackgroundJob = await ClientDbContext.SysBackgroundJobs.FindAsync(sysJobsId);
            return mapper.Map<SysBackgroundJobsDto>(sysBackgroundJob);
        }
        public async Task CreateSysBackgroundJobAsync(SysBackgroundJobsDto sysBackgroundJobsDto)
        {
            var sysBackgroundJob = mapper.Map<SysBackgroundJob>(sysBackgroundJobsDto);
            ClientDbContext.SysBackgroundJobs.Add(sysBackgroundJob);
            await ClientDbContext.SaveChangesAsync();
        }



    }
}
