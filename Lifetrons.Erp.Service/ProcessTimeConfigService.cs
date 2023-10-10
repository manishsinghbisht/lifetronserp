using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Service
{
    public class ProcessTimeConfigService : Service<ProcessTimeConfig>, IProcessTimeConfigService
    {
        private readonly IRepositoryAsync<ProcessTimeConfig> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public ProcessTimeConfigService(IRepositoryAsync<ProcessTimeConfig> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<ProcessTimeConfig>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Process)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<ProcessTimeConfig> Select(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Process)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public IEnumerable<ProcessTimeConfig> Select(string processId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid processIdGuid = processId.ToSysGuid();

            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.ProcessId == processIdGuid)
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Process)
              .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public async Task<ProcessTimeConfig> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var processTimeConfig = await _repository.FindAsync(id.ToSysGuid());
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (processTimeConfig == null) return null;
            if (processTimeConfig.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return processTimeConfig;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProcessTimeConfig Create(ProcessTimeConfig param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            //Validate
            var applicationUser = _aspNetUserService.Find(userId);
            Validate(param, applicationUser);
           
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        private void Validate(ProcessTimeConfig param, AspNetUser applicationUser)
        {
            //Job dates should not exceed parent prodplan dates
            //Check start and end dates
            if (param.FromDate >= param.ToDate) throw new ApplicationException("From (Start) date time should be less than To (End) date time");

            //Check start and end dates
            if (param.StartTime >= param.EndTime) throw new ApplicationException("Start time should be less than To End time");

        }
        public void Update(ProcessTimeConfig param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            
            var applicationUser = _aspNetUserService.Find(userId);
            //Business validation
            Validate(param, applicationUser);

            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        public void Dispose()
        {
        }

    }
}
