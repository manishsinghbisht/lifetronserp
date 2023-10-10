using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> SelectAsyncContactsByAccount(string accountId, string orgId);
        Task<IEnumerable<Contact>> SelectAsync(string userId, string orgId);
        Task<IEnumerable<Contact>> SelectAsyncAccountNotNull(string orgId);
        Task<IEnumerable<Contact>> FindAsyncByName(string userId, string orgId, string name);
        Task<IEnumerable<Contact>> FindAsyncByEmail(string email);
        IEnumerable<Contact> FindByEmail(string email);
        Contact Find(string id, string userId, string orgId);
        Task<Contact> FindAsync(string id, string userId, string orgId);
        Contact Create(Contact param, string userId, string orgId);
        void Update(Contact param, string userId, string orgId);
        IEnumerable<Contact> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}