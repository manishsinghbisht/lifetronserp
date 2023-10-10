
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    using Lifetrons.Erp.Data;
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CampaignMemberStatusService : Service<CampaignMemberStatu>, ICampaignMemberStatusService
    {
        private readonly IRepositoryAsync<CampaignMemberStatu> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public CampaignMemberStatusService(IRepositoryAsync<CampaignMemberStatu> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public IEnumerable<CampaignMemberStatu> SelectLineItems(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = _repository.Query()
                .OrderBy(q => q.OrderBy(c => c.Serial))
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<CampaignMemberStatu>> SelectAsyncLineItems(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = await _repository.Query()
                .OrderBy(q => q.OrderBy(c => c.Serial))
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<CampaignMemberStatu> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = await _repository.FindAsync(id.ToSysGuid());

            return task;
        }

        public void Dispose()
        {
        }

    }
}
