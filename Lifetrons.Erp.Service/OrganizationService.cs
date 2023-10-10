using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class OrganizationService : Service<Organization>, IOrganizationService
    {
        private readonly IRepositoryAsync<Organization> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public OrganizationService(IRepositoryAsync<Organization> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;

        }

        public async Task<IEnumerable<Organization>> GetAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var task = await _repository.Query(p => p.Id.Equals(orgIdGuid) & p.Active == true)
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            var firstOrDefault = task.FirstOrDefault();
            if (firstOrDefault == null)
            {
                return null;
            }
            if (firstOrDefault.Id == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Organization> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var task = await _repository.FindAsync(id.ToSysGuid());
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (task.Id == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Organization Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var organization = _repository.Find(id.ToSysGuid());
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (organization.Id == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return organization;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Organization Create(Organization param)
        {
            if (param == null)
                throw new ApplicationException("Parameters cannot be null or empty");
            _repository.Insert(param);
            return param;
        }

        public void Update(Organization param)
        {
            //Do the validation here

            //Do business logic here

            //Update
            _repository.Update(param);
        }

        public async Task<IEnumerable<Organization>> GetAsyncOrganizations(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var applicationUser = _aspNetUserService.SelectSingleUserWithRoles(userId, orgId);
            if (applicationUser.AspNetRoles.Any(role => (role.Name == "SuperAdmin")))
            {
                return await _repository.Query()
                    .Include(p => p.AspNetUser)
                    .Include(p => p.AspNetUser1)
                    .SelectAsync();
            }

            return null;
        }

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        //public IEnumerable<Organization> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        //{
        //    var records = _repository
        //        .Query()
        //        .OrderBy(q => q
        //            .OrderBy(c => c.Name)
        //            .ThenBy(c => c.City))
        //        .Filter(q => !string.IsNullOrEmpty(q.Name))
        //        .GetPage(pageNumber, pageSize, out totalRecords);

        //    return records;
        //}
        public void Dispose()
        {
        }

    }
}
