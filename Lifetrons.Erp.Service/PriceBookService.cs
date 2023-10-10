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

    public class PriceBookService : Service<PriceBook>, IPriceBookService
    {
        private readonly IRepositoryAsync<PriceBook> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public PriceBookService(IRepositoryAsync<PriceBook> repository, IAspNetUserService aspNetUserService)
            : base (repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public IEnumerable<PriceBook> Select(string userId, string orgId)
        {
            Guid orgIdGuid;
            if (ValidateParams(userId, orgId, out orgIdGuid)) return null;
            var applicationUser = _aspNetUserService.SelectSingleUserWithRoles(userId, orgId);
            IEnumerable<PriceBook> task;

            if (applicationUser.AspNetRoles.Any(role => (role.Name == "OrganizationLevel" || role.Name == "PriceBookManager")))
            {
                task = _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
                   .Include(p => p.AspNetUser)
                   .Include(p => p.AspNetUser1)
                   .Include(p => p.Organization)
                   .Select();
            }
            else
            {
                task = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.SharedWith == null || p.SharedWith.Contains(applicationUser.UserName)))
                    .Include(p => p.AspNetUser)
                    .Include(p => p.AspNetUser1)
                    .Include(p => p.Organization)
                    .Select();
            }

            //Check user & org
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<PriceBook>> SelectAsync(string userId, string orgId)
        {
            Guid orgIdGuid;
            if (ValidateParams(userId, orgId, out orgIdGuid)) return null;
            var applicationUser = _aspNetUserService.SelectSingleUserWithRoles(userId, orgId);
            IEnumerable<PriceBook> task;

            if (applicationUser.AspNetRoles.Any(role => (role.Name == "OrganizationLevel" || role.Name == "PriceBookManager")))
            {
                task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
                 .Include(p => p.AspNetUser)
                 .Include(p => p.AspNetUser1)
                 .Include(p => p.Organization)
                 .SelectAsync();
            }
            else
            {
                task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & (p.SharedWith == null || p.SharedWith.Contains(applicationUser.UserName)))
                   .Include(p => p.AspNetUser)
                   .Include(p => p.AspNetUser1)
                   .Include(p => p.Organization)
                   .SelectAsync();
            }

            //Check user & org
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        private static bool ValidateParams(string userId, string orgId, out Guid orgIdGuid)
        {
            orgIdGuid = orgId.ToSysGuid();
            if (string.IsNullOrEmpty(userId)) return true;

            return false;
        }

        public PriceBook Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var task = _repository.Find(id.ToSysGuid());

            //Check user, org, & sharing
            var applicationUser = _aspNetUserService.SelectSingleUserWithRoles(userId, orgId);
            if (!(task.SharedWith == null || task.SharedWith.Contains(applicationUser.UserName)) &&
                !(applicationUser.AspNetRoles.Any(role => (role.Name == "OrganizationLevel" || role.Name == "PriceBookManager"))))
            {
                return null;
            }

            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<PriceBook> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
            Guid orgIdGuid = orgId.ToSysGuid();
            var task = await _repository.FindAsync(id.ToSysGuid());

            //Check user, org, & sharing
            var applicationUser = _aspNetUserService.SelectSingleUserWithRoles(userId, orgId);
            if (!(task.SharedWith == null || task.SharedWith.Contains(applicationUser.UserName)) &&
                !(applicationUser.AspNetRoles.Any(role => (role.Name == "OrganizationLevel" || role.Name == "PriceBookManager"))))
            {
                return null;
            }

            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public PriceBook Create(PriceBook param, string userId, string orgId)
        {
            if (param.Name == "DEFAULT" || param.Code == "DEFAULT")
            {
                throw new Exception("DEFAULT as Name or Code is InValid");
            }

            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(PriceBook param, string userId, string orgId)
        {
            if (param.Name == "DEFAULT" || param.Code == "DEFAULT")
            {
                throw new Exception("DEFAULT as Name or Code is InValid");
            }

            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
           
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
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

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        public IEnumerable<PriceBook> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Name) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Name)
                        .ThenBy(c => c.Code))
                    .SelectPage(pageNumber, pageSize, out totalRecords);

                return records;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public void Dispose()
        {
        }

    }
}
