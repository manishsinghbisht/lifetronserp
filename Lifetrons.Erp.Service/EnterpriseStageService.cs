using System.ComponentModel;
using Lifetrons.Erp.Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class EnterpriseStageService : Service<EnterpriseStage>, IEnterpriseStageService
    {
        private readonly IRepositoryAsync<EnterpriseStage> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public EnterpriseStageService(IRepositoryAsync<EnterpriseStage> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<EnterpriseStage>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var enterpriseStages = await _repository.Query()
                                .Include(p => p.Department)
                                .OrderBy(p => p.OrderBy(c => c.Serial).ThenBy(c => c.Name))
                                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enterpriseStages;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<EnterpriseStage> Select(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var enterpriseStages = _repository.Query()
                                                .Include(p => p.Department)
                                                .OrderBy(p => p.OrderBy(c => c.Serial).ThenBy(c => c.Name))
                                                .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enterpriseStages;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<EnterpriseStage> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var enterpriseStage = await _repository.FindAsync(id.ToSysGuid());
            
            return enterpriseStage;
        }

        public EnterpriseStage Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var enterpriseStage = _repository.Find(id.ToSysGuid());

            return enterpriseStage;
        }

       
       public void Dispose()
        {
        }

    }
}
