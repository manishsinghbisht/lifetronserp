using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IItemService
    {
        IEnumerable<Item> Select(string userId, string orgId);
        Task<IEnumerable<Item>> SelectAsync(string userId, string orgId);
        Task<IEnumerable<Item>> SelectAsync(string userId, string orgId, List<Guid> containsIds);
        Task<Item> FindAsync(string id, string userId, string orgId);
        Item Find(string id, string userId, string orgId);
        Item Create(Item param, string userId, string orgId);
        void Update(Item param, string userId, string orgId);
        IEnumerable<Item> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}