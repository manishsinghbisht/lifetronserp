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
    public class ContactService : Service<Contact>, IContactService
    {
        private readonly IRepositoryAsync<Contact> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;

        public ContactService(IRepositoryAsync<Contact> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public async Task<IEnumerable<Contact>> SelectAsyncContactsByAccount(string accountId, string orgId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid accountIdGuid = accountId.ToSysGuid();

            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.AccountId == accountIdGuid)
                .OrderBy(q => q
                        .OrderBy(c => c.Name)
                        .ThenBy(c => c.Code))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.AspNetUser3)
              .Include(p => p.Account)
              .Include(p => p.Level)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.LeadSource)
              .Include(p => p.Contact1) //Reports to
              .SelectAsync();

            return task;
        }

        public async Task<IEnumerable<Contact>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
              .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.AspNetUser3)
              .Include(p => p.Account)
              .Include(p => p.Level)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.LeadSource)
              .Include(p => p.Contact1) //Reports to
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Contact>> SelectAsyncAccountNotNull(string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) return null;
            Guid orgIdGuid = orgId.ToSysGuid();

            var enumerable = await _repository.Query(p => p.Active & p.AccountId != null & p.OrgId == orgIdGuid)
              .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.AspNetUser3)
              .Include(p => p.Account)
              .Include(p => p.Level)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.LeadSource)
              .Include(p => p.Contact1) //Reports to
              .SelectAsync();

            return enumerable;
        }

        public async Task<IEnumerable<Contact>> FindAsyncByName(string userId, string orgId, string name)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Name.Contains(name))
              .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.AspNetUser3)
              .Include(p => p.Account)
              .Include(p => p.Level)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.LeadSource)
              .Include(p => p.Contact1) //Reports to
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Contact>> FindAsyncByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var enumerable = await _repository.Query(p => p.Active & (p.PrimaryEMail.Contains(email) || p.OtherAddressEMail.Contains(email) || p.MailingAddressEMail.Contains(email)))
              .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.AspNetUser3)
              .Include(p => p.Account)
              .Include(p => p.Level)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.LeadSource)
              .Include(p => p.Contact1) //Reports to
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<Contact> FindByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            var lowerdEmail = email.ToLower();
            var enumerable = _repository.Query(p => p.Active & (p.PrimaryEMail.ToLower() == lowerdEmail || p.OtherAddressEMail.ToLower() == lowerdEmail || p.MailingAddressEMail.ToLower() == lowerdEmail))
              .OrderBy(q => q.OrderBy(c => c.Name).ThenBy(c => c.Code))
              .Include(p => p.AspNetUser)
              .Include(p => p.AspNetUser1)
              .Include(p => p.AspNetUser2)
              .Include(p => p.AspNetUser3)
              .Include(p => p.Account)
              .Include(p => p.Level)
              .Include(p => p.Address)
              .Include(p => p.Address1)
              .Include(p => p.LeadSource)
              .Include(p => p.Contact1) //Reports to
              .Select();

            return enumerable;
        }

        public Contact Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var contact = _repository.Find(id.ToSysGuid());
            if (contact == null) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (contact.OrgId == applicationUser.OrgId)
            {
                return contact;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Contact> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var contact = await _repository.FindAsync(id.ToSysGuid());
            if (contact == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (contact.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                contact.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return contact;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Contact Create(Contact param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Contact param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

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

        public IEnumerable<Contact> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

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
