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
    public class PriceBookLineItemService : Service<PriceBookLineItem>, IPriceBookLineItemService
    {
        private readonly IRepositoryAsync<PriceBookLineItem> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IRepositoryAsync<PriceBook> _priceBookRepository;
        private readonly IRepositoryAsync<Product> _productRepository;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public PriceBookLineItemService(IRepositoryAsync<PriceBookLineItem> repository, IAspNetUserService aspNetUserService,
            IRepositoryAsync<PriceBook> priceBookRepository, IRepositoryAsync<Product> productRepository, IOrganizationService organizationService)
            : base (repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _priceBookRepository = priceBookRepository;
            _productRepository = productRepository;
            _organizationService = organizationService;
        }

        //public async Task<IEnumerable<PriceBookLineItem>> GetAsync(string userId, string orgId)
        //{
        //    Guid orgIdGuid = orgId.ToSysGuid();
        //    if (orgIdGuid == Guid.Empty)
        //    {
        //        throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
        //    }
        //    var task = await _repository.Query()
        //       .Include(p => p.AspNetUser)
        //       .Include(p => p.AspNetUser1)
        //       .Include(p => p.PriceBook)
        //       .Include(p => p.Product)
        //       .Filter(p => p.Active & p.OrgId == orgIdGuid)
        //       .GetAsync();

        //    //Check user & org
        //    var applicationUser = await _aspNetUserService.FindAsync(userId);
        //    if (orgIdGuid == applicationUser.OrgId)
        //    {
        //        return task;
        //    }
        //    throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        //}
        public IEnumerable<PriceBookLineItem> SelectLineItems(string priceBookId, string userId, string orgId)
        {
            Guid orgIdGuid;
            Guid listIdGuid;
            if (ValidateParams(priceBookId, userId, orgId, out orgIdGuid, out listIdGuid)) return null;

            var task = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.PriceBookId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(p => p.OrderBy(c => c.Product.Name).ThenBy(c => c.Product.Code))
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public async Task<IEnumerable<PriceBookLineItem>> SelectAsyncLineItems(string priceBookId, string userId, string orgId)
        {
            Guid orgIdGuid;
            Guid listIdGuid;
            if (ValidateParams(priceBookId, userId, orgId, out orgIdGuid, out listIdGuid)) return null;

            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.PriceBookId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(p => p.OrderBy(c => c.Product.Name).ThenBy(c => c.Product.Code))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public List<PriceBookLineItem> GetDefaultPriceBook(string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            
            var task = _repository.Query(p => p.OrgId == orgIdGuid & p.PriceBook.Name == "DEFAULT" & p.PriceBook.Code == "DEFAULT")
               .Include(p => p.Organization)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .OrderBy(p => p.OrderBy(c => c.Product.Name).ThenBy(c => c.Product.Code))
               .Select();

            return task.ToList();
        }

        private static bool ValidateParams(string priceBookId, string userId, string orgId, out Guid orgIdGuid,
            out Guid listIdGuid)
        {
            orgIdGuid = orgId.ToSysGuid();
            listIdGuid = priceBookId.ToSysGuid();

            if (string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return true;

            return false;
        }

        public PriceBookLineItem Find(string priceBookId, string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();

            var lineItem = _repository.Find(new object[] { priceBookId.ToSysGuid(), productId.ToSysGuid() });
            if (lineItem == null) return lineItem;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (lineItem.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                var priceBook =_priceBookRepository.Find(new object[] {priceBookId.ToSysGuid()});
                var product = _productRepository.Find(new object[] { productId.ToSysGuid() });
                lineItem.PriceBook = priceBook;
                lineItem.Product = product;
                lineItem.Organization = _organizationService.Find(orgId, userId, orgId);
                return lineItem;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<PriceBookLineItem> FindAsync(string priceBookId, string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();

            var task = await _repository.FindAsync(new object[] { priceBookId.ToSysGuid(), productId.ToSysGuid() });
            
            if (task == null) return task;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            
            
            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                var priceBook = _priceBookRepository.Find(new object[] { priceBookId.ToSysGuid() });
                var product = _productRepository.Find(new object[] { productId.ToSysGuid() });
                task.PriceBook = priceBook;
                task.Product = product;
                task.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }


        public PriceBookLineItem Create(PriceBookLineItem param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(PriceBookLineItem param, string userId, string orgId)
        {
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

        public IEnumerable<PriceBookLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
           var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Product.Code))
                    .OrderBy(q => q
                        .OrderBy(c => c.PriceBook)
                        .ThenBy(c => c.Product))
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
