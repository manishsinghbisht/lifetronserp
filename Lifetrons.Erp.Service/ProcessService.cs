using System.Collections;
using System.Linq;
using Lifetrons.Erp.Data;
using Microsoft.Practices.Unity;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Service
{
    public class ProcessService : Service<Process>, IProcessService
    {
        [Dependency]
        public IProcessTimeConfigService ProcessTimeConfigService { get; set; }

        private readonly IRepositoryAsync<Process> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public ProcessService(IRepositoryAsync<Process> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Process>> SelectEnterpriseStageProcessAsync(string enterpriseStageId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
            Guid enterpriseStageIdGuid = enterpriseStageId.ToSysGuid();
            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var processes = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.EnterpriseStageId == enterpriseStageIdGuid)
               .OrderBy(p => p.OrderBy(c => c.Serial).ThenBy(c => c.Name))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.EnterpriseStage)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return processes;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Process>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var processes = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .OrderBy(p => p.OrderBy(c => c.Serial).ThenBy(c => c.Name))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.EnterpriseStage)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return processes;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Process>> SelectAsyncForJobs(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var productionStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Production"].ToSysGuid();
            var stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();

            var processes = (await _repository.Query(p => p.Active & p.OrgId == orgIdGuid
                & (p.EnterpriseStageId.Equals(productionStageGuid) | p.EnterpriseStageId.Equals(stockStageGuid)))
              .OrderBy(p => p.OrderBy(c => c.EnterpriseStageId).ThenBy(c => c.Serial))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.EnterpriseStage)
              .SelectAsync()).ToList();

            //var removeList = new List<Process>();
            //removeList.Add(processes.Find(p => p.EnterpriseStageId.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Logistics"].ToSysGuid())));
            //removeList.Add(processes.Find(p => p.EnterpriseStageId.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Planning"].ToSysGuid())));
            //removeList.Add(processes.Find(p => p.EnterpriseStageId.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Procurement"].ToSysGuid())));

            //removeList.ForEach(p => processes.Remove(p));

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return processes;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Process>> SelectAsyncStockProcesses(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();

            var processes = (await _repository.Query(p => p.Active & p.OrgId == orgIdGuid
                & (p.EnterpriseStageId.Equals(stockStageGuid)))
              .OrderBy(p => p.OrderBy(c => c.EnterpriseStageId).ThenBy(c => c.Serial))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.Organization)
              .Include(p => p.EnterpriseStage)
              .SelectAsync()).ToList();

            //var removeList = new List<Process>();
            //removeList.Add(processes.Find(p => p.EnterpriseStageId.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Logistics"].ToSysGuid())));
            //removeList.Add(processes.Find(p => p.EnterpriseStageId.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Planning"].ToSysGuid())));
            //removeList.Add(processes.Find(p => p.EnterpriseStageId.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Procurement"].ToSysGuid())));

            //removeList.ForEach(p => processes.Remove(p));

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return processes;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Process> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var process = await _repository.FindAsync(id.ToSysGuid());

            if (process != null)
            {
                process.ProcessTimeConfigs = ProcessTimeConfigService.Select(id, userId, orgId).ToList();
            }

            return process;
        }

        public Process Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var process = _repository.Find(id.ToSysGuid());

            if (process != null)
            {
                process.ProcessTimeConfigs = ProcessTimeConfigService.Select(id, userId, orgId).ToList();
            }

            return process;
        }

        public Process Create(Process param, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                if (param.CycleTimeInHour <= 0)
                {
                    param.CycleTimeInHour = (Decimal)0.0166667; //1 minute = 0.0166667 hour
                }
                if (param.CycleCapacity <= 0)
                {
                    param.CycleCapacity = 1;
                }

                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Process param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business cases
                if (param.CycleTimeInHour <= 0)
                {
                    param.CycleTimeInHour = (Decimal)0.0166667; //1 minute = 0.0166667 hour
                }
                if (param.CycleCapacity <= 0)
                {
                    param.CycleCapacity = 1;
                }
                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public void Dispose()
        {
        }

    }
}
