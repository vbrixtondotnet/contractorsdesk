using ContractorsDesk.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
    public interface ISysBackgroundJobsService
    {
        Task<IEnumerable<SysBackgroundJobsDto>> GetAllSysBackgroundJobsAsync();
        Task<SysBackgroundJobsDto> GetSysBackgroundJobAsync(int sysJobsId);
        Task CreateSysBackgroundJobAsync(SysBackgroundJobsDto sysBackgroundJobsDto);


    }
}
