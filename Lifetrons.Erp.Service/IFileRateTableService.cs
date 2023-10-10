namespace Lifetrons.Erp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lifetrons.Erp.Data;
    using System;

    public interface IFileRateTableService
    {
        Task<IEnumerable<FileRateTable>> FindAsyncByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime);
        IEnumerable<FileRateTable> FindByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime);
        IEnumerable<FileRateTable> FindByContact(string fileType, string contactId, DateTime startDateTime, DateTime endDateTime);
        FileRateTable Find(string id);
        Task<FileRateTable> FindAsync(string id);
        FileRateTable Create(FileRateTable param, string userId, string orgId);
        void Update(FileRateTable param, string userId, string orgId);
        Task<IEnumerable<FileRateTable>> GetAsync(string orgId);
        void Delete(string id);
        IEnumerable<FileRateTable> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}