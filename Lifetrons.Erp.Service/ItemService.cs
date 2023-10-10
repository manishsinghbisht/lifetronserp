using System;
using Lifetrons.Erp.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class ItemService : Service<Item>, IItemService
    {
        private readonly IRepositoryAsync<Item> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public ItemService(IRepositoryAsync<Item> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<Item> Select(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var items = _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.ItemClassification)
               .Include(p => p.ItemCategory)
               .Include(p => p.ItemType)
               .Include(p => p.ItemGroup)
               .Include(p => p.ItemSubGroup)
               .Include(p => p.CostingGroup)
               .Include(p => p.CostingSubGroup)
               .Include(p => p.Nature)
               .Include(p => p.Shape)
               .Include(p => p.Colour)
               .Include(p => p.Style)
               .Include(p => p.WeightUnit)
               .Include(p => p.WeightUnit)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return items;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Item>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var items = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.Organization)
                .Include(p => p.ItemClassification)
                .Include(p => p.ItemCategory)
                .Include(p => p.ItemType)
                .Include(p => p.ItemGroup)
                .Include(p => p.ItemSubGroup)
                .Include(p => p.CostingGroup)
                .Include(p => p.CostingSubGroup)
                .Include(p => p.Nature)
                .Include(p => p.Shape)
                .Include(p => p.Colour)
                .Include(p => p.Style)
                .Include(p => p.WeightUnit)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return items;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Item>> SelectAsync(string userId, string orgId, List<Guid> containsIds)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var items = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & containsIds.Contains(p.Id))
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.Organization)
                .Include(p => p.ItemClassification)
                .Include(p => p.ItemCategory)
                .Include(p => p.ItemType)
                .Include(p => p.ItemGroup)
                .Include(p => p.ItemSubGroup)
                .Include(p => p.CostingGroup)
                .Include(p => p.CostingSubGroup)
                .Include(p => p.Nature)
                .Include(p => p.Shape)
                .Include(p => p.Colour)
                .Include(p => p.Style)
                .Include(p => p.WeightUnit)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return items;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Item> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var item = await _repository.FindAsync(id.ToSysGuid());
            if (item == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (item.OrgId == applicationUser.OrgId)
            {
                item.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return item;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Item Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var item = _repository.Find(id.ToSysGuid());
            if (item == null) return null;
            
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (item.OrgId == applicationUser.OrgId)
            {
                item.Organization = _organizationService.Find(orgId, userId, orgId);
                return item;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Item Create(Item param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Item param, string userId, string orgId)
        {
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

        public IEnumerable<Item> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
                    .Query(q => !string.IsNullOrEmpty(q.Name))
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
