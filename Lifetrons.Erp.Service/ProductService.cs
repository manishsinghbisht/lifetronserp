using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;
using Repository.Pattern.Ef6;
using Microsoft.Practices.Unity;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Service
{
    public class ProductService : Service<Product>, IProductService
    {
        [Dependency]
        public Repository<PriceBook> PriceBookRepository { get; set; }

        [Dependency]
        public Repository<PriceBookLineItem> PriceBookLineItemRepository { get; set; }

        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }

        private readonly IRepositoryAsync<Product> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public ProductService(IRepositoryAsync<Product> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<Product> Select(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var products = _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.ProductFamily)
               .Include(p => p.ProductType)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return products;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Product>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var products = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.ProductFamily)
               .Include(p => p.ProductType)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return products;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Product> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var product = await _repository.FindAsync(id.ToSysGuid()); //new object[] { productId.ToSysGuid(), enterpriseStageId.ToSysGuid(), processId.ToSysGuid() }
            if (product == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (product.OrgId == applicationUser.OrgId)
            {
                product.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return product;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Product Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var product = _repository.Find(id.ToSysGuid()); //new object[] { productId.ToSysGuid(), enterpriseStageId.ToSysGuid(), processId.ToSysGuid() }
            if (product == null) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (product.OrgId == applicationUser.OrgId)
            {
                product.Organization = _organizationService.Find(orgId, userId, orgId);
                return product;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public Product Create(Product param, string userId, string orgId)
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
                AddProductToDefaultPriceBook(param.Id, userId, orgIdGuid);
                
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public bool AddProductToDefaultPriceBook(Guid productId, string userId, Guid orgId)
        {
            var priceBooks = PriceBookRepository.Query(p => p.OrgId == orgId & p.Code == "DEFAULT" & p.Name == "DEFAULT")
                .OrderBy(q => q.OrderByDescending(c => c.CreatedDate))
                .Select();

            if (priceBooks == null)
            {
                CreateDefaultPriceBookEntry(productId, userId, orgId);
            }
            else if (priceBooks.Count() <= 0)
            {
                CreateDefaultPriceBookEntry(productId, userId, orgId);
            }
            else
            {
                var defaultPriceBook = priceBooks.FirstOrDefault();

                PriceBookLineItem priceBookLineItem = new PriceBookLineItem();
                priceBookLineItem.Id = Guid.NewGuid();
                priceBookLineItem.PriceBookId = defaultPriceBook.Id;
                priceBookLineItem.ProductId = productId;
                priceBookLineItem.ListPrice = 1;
                priceBookLineItem.OrgId = orgId;
                priceBookLineItem.CreatedBy = userId;
                priceBookLineItem.ModifiedBy = userId;
                priceBookLineItem.CreatedDate = DateTime.UtcNow;
                priceBookLineItem.ModifiedDate = DateTime.UtcNow;
                priceBookLineItem.Active = true;
                priceBookLineItem.Authorized = true;

                PriceBookLineItemRepository.Insert(priceBookLineItem);
            }

            return true;
        }

        public bool CreateDefaultPriceBookEntry(Guid productId, string userId, Guid orgId)
        {
            //// Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                PriceBook defaultPriceBook = new PriceBook();
                defaultPriceBook.Code = "DEFAULT";
                defaultPriceBook.Name = "DEFAULT";
                defaultPriceBook.Id = Guid.NewGuid();
                defaultPriceBook.OrgId = orgId;
                defaultPriceBook.CreatedBy = userId;
                defaultPriceBook.ModifiedBy = userId;
                defaultPriceBook.CreatedDate = DateTime.UtcNow;
                defaultPriceBook.ModifiedDate = DateTime.UtcNow;
                defaultPriceBook.Active = true;
                defaultPriceBook.Authorized = true;

                PriceBookRepository.Insert(defaultPriceBook);

                PriceBookLineItem priceBookLineItem = new PriceBookLineItem();
                priceBookLineItem.Id = Guid.NewGuid();
                priceBookLineItem.PriceBookId = defaultPriceBook.Id;
                priceBookLineItem.ProductId = productId;
                priceBookLineItem.ListPrice = 1;
                priceBookLineItem.OrgId = orgId;
                priceBookLineItem.CreatedBy = userId;
                priceBookLineItem.ModifiedBy = userId;
                priceBookLineItem.CreatedDate = DateTime.UtcNow;
                priceBookLineItem.ModifiedDate = DateTime.UtcNow;
                priceBookLineItem.Active = true;
                priceBookLineItem.Authorized = true;

                PriceBookLineItemRepository.Insert(priceBookLineItem);

                UnitOfWork.SaveChanges();

                //// Commit Transaction
                UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                //// Rollback transaction
                UnitOfWork.Rollback();

                throw exception;
            }

            return true;
        }

        public void Update(Product param, string userId, string orgId)
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

        public IEnumerable<Product> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
