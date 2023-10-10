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
    public interface IEnterpriseStageService : IService<EnterpriseStage>
    {
        Task<IEnumerable<EnterpriseStage>> SelectAsync(string userId, string orgId);
        Task<EnterpriseStage> FindAsync(string id, string userId, string orgId);
        IEnumerable<EnterpriseStage> Select(string userId, string orgId);
        EnterpriseStage Find(string id, string userId, string orgId);
        void Dispose();
    }
}