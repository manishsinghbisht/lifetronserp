using Lifetrons.Erp.Data;
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
    public class AspNetUserService : Service<AspNetUser>, IAspNetUserService
    {
        private readonly IRepositoryAsync<AspNetUser> _repository;

        public AspNetUserService(IRepositoryAsync<AspNetUser> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<AspNetUser>> GetAsyncUsers(string userId, string orgId)
        {
            var applicationUser = SelectSingleUserWithRoles(userId, orgId);

            if (
                applicationUser.AspNetRoles.Any(role => (role.Name == "SuperAdmin")))
            {
                return _repository.Query()
                    .Include(d => d.Organization)
                    .Include(d => d.AspNetRoles)
                   .SelectAsync();
            }
            return null;
        }

        public Task<IEnumerable<AspNetUser>> SelectAsync(string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            return _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
               .Include(d => d.Organization)
               .Include(d => d.AspNetRoles)
                .SelectAsync();
        }

        public AspNetUser SelectSingleUserWithRoles(string id, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            
            var user = _repository.Query(p => p.Active & p.Id == id & p.OrgId == orgIdGuid)
              .Include(d => d.Organization)
              .Include(d => d.AspNetRoles)
               .Select().Single();

            //var user = userQueryResult.Result.First(u => u.Id == id);

            return user;
        }

        public AspNetUser FindByUserName(string userName)
        {
            var user = _repository.Query(p => p.Active & p.UserName == userName)
               .Select().Single();

            //var user = userQueryResult.Result.First(u => u.Id == id);

            return user;
        }

        public Task<AspNetUser> FindAsync(string id)
        {
            return _repository.FindAsync(id);
        }

        public AspNetUser Find(string id)
        {
            return _repository.Find(id);
        }

        //public IEnumerable<AspNetUser> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        //{
        //    var records = _repository
        //        .Query()
        //        .OrderBy(q => q
        //            .OrderBy(c => c.UserName)
        //            .ThenBy(c => c.AuthenticatedEmail))
        //        .Filter(q => !string.IsNullOrEmpty(q.UserName))
        //        .GetPage(pageNumber, pageSize, out totalRecords);

        //    return records;
        //}
    }
}
