using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersByAccountAsync(DateTime startDateTime, DateTime endDateTime, string accountId,
            string orgId);

        Task<IEnumerable<Order>> GetOrdersBySubAccountAsync(DateTime startDateTime, DateTime endDateTime,
            string subAccountId, string orgId);
        IEnumerable<Order> GetOrdersByUserList(DateTime startDateTime, DateTime endDateTime, string userList,
            string userId, string orgId);

        IEnumerable<Order> GetOrdersByOrganization(DateTime startDateTime, DateTime endDateTime, string userId,
            string orgId);

        IEnumerable<Order> GetOrdersByOwner(DateTime startDateTime, DateTime endDateTime, string ownerId, string orgId);

        IEnumerable<Order> Select(string orderId, string userId, string orgId);
        Task<IEnumerable<Order>> SelectAsync(string userId, string orgId);

        Task<IEnumerable<Order>> SelectAsyncAllOrders(string userId, string orgId);

        IEnumerable<Order> GetOrdersByAmountAndDateRange(DateTime fromDateTime, DateTime toDateTime, decimal amount,
            string country);

        void Delete(Order model);
        Order Find(string id, string userId, string orgId);
        Task<Order> FindAsync(string id, string userId, string orgId);
        Order Create(Order param, string userId, string orgId);
        void Update(Order param, string userId, string orgId);
        IEnumerable<Order> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}