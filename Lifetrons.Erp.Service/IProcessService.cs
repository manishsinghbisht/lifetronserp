using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public interface IProcessService : IService<Process>
    {
        Task<IEnumerable<Process>> SelectEnterpriseStageProcessAsync(string enterpriseStageId, string userId,
            string orgId);
        Task<IEnumerable<Process>> SelectAsync(string userId, string orgId);
        Task<IEnumerable<Process>> SelectAsyncForJobs(string userId, string orgId);
        Task<IEnumerable<Process>> SelectAsyncStockProcesses(string userId, string orgId);
        Task<Process> FindAsync(string id, string userId, string orgId);
        Process Create(Process param, string userId, string orgId);
        void Update(Process param, string userId, string orgId);
        Process Find(string id, string userId, string orgId);
        void Delete(string id);
        void Dispose();
    }
}