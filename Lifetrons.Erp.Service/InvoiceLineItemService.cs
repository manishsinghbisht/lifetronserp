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
    public class InvoiceLineItemService : Service<InvoiceLineItem>, IInvoiceLineItemService
    {
        private readonly IRepositoryAsync<InvoiceLineItem> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public InvoiceLineItemService(IRepositoryAsync<InvoiceLineItem> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public IEnumerable<InvoiceLineItem> SelectLineItems(string invoiceId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(invoiceId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = invoiceId.ToSysGuid();
            var task = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.InvoiceId == listIdGuid)
                .Include(p => p.Organization)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Invoice)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q.OrderBy(c => c.Serial).ThenBy(c => c.Product.Name))
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<InvoiceLineItem>> SelectAsyncLineItems(string invoiceId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(invoiceId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid listIdGuid = invoiceId.ToSysGuid();
            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.InvoiceId == listIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Invoice)
               .Include(p => p.PriceBook)
               .Include(p => p.Product)
               .Include(p => p.WeightUnit)
               .OrderBy(q => q.OrderBy(c => c.Serial).ThenBy(c => c.Product.Name))
               .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<InvoiceLineItem> FindAsync(string invoiceId, string priceBookId, string productId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            var orgIdGuid = orgId.ToSysGuid();
            var task = await _repository.FindAsync(new object[] { invoiceId.ToSysGuid(), priceBookId.ToSysGuid(), productId.ToSysGuid() });
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (task == null) return task;
            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }


        public InvoiceLineItem Create(InvoiceLineItem param, string userId, string orgId)
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

        public void Update(InvoiceLineItem param, string userId, string orgId)
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

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<InvoiceLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);

            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.Product.Code))
                    .OrderBy(q => q
                        .OrderBy(c => c.InvoiceId)
                        .ThenBy(c => c.PriceBook)
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
