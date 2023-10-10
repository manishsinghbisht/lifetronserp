using System;
using System.Security.Cryptography.X509Certificates;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class ProductTypeService : Service<ProductType>, IProductTypeService
    {
        private readonly IRepositoryAsync<ProductType> _repository;
        private readonly IAspNetUserService _aspNetUserService;

        public ProductTypeService(IRepositoryAsync<ProductType> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<ProductType>> GetAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<ProductType> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var task = await _repository.FindAsync(id.ToSysGuid());
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (task == null) return task;
            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public ProductType Create(ProductType param, string userId, string orgId)
        {
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

        public void Update(ProductType param, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
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

        public IEnumerable<ProductType> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
