namespace Lifetrons.Erp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lifetrons.Erp.Data;
    using System;

    public interface IFileService
    {
        Task<IEnumerable<File>> FindAsyncByStatus(string fileType, string status, string orgId);
        IEnumerable<File> FindByStatus(string fileType, string status, string orgId);
        Task<IEnumerable<File>> FindAsyncByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime, string status);
        IEnumerable<File> FindByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime, string status);
        Task<IEnumerable<File>> FindAsyncByAccount(string accountId, DateTime startDateTime, DateTime endDateTime, string status);
        IEnumerable<File> FindByAccount(string accountId, DateTime startDateTime, DateTime endDateTime, string status);
        IEnumerable<File> FindDeliveriesByProcessor(string fileType, string processorId, DateTime startDateTime, DateTime endDateTime);
        IEnumerable<File> FindDeliveriesByDate(string fileType, DateTime startDateTime, DateTime endDateTime, string orgId);
        Task<IEnumerable<File>> FindAsyncByProcessor(string fileType, string processorId, DateTime startDateTime, DateTime endDateTime, string status);
        IEnumerable<File> FindByProcessor(string fileType, string processorId, DateTime startDateTime, DateTime endDateTime, string status);
        Task<IEnumerable<File>> FindAsyncByProcessor(string processorId, DateTime startDateTime, DateTime endDateTime, string status);
        IEnumerable<File> FindByProcessor(string processorId, DateTime startDateTime, DateTime endDateTime, string status);
        IEnumerable<File> FindByUserAndStatus(string fileType, string status, string userId);
        bool AssignFileToUser(string userId, out string message);
        File FindRecentFile(string fileType, string orgId);
        File Find(string id);
        Task<File> FindAsync(string id);
        File Create(File param, string userId, string orgId);
        void Update(File param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<File> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}