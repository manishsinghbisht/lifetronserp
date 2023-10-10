using Lifetrons.Erp.Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;
using Repository.Pattern.UnitOfWork;
using Task = System.Threading.Tasks.Task;


namespace Lifetrons.Erp.Service
{
    public class FileService : Service<File>, IFileService
    {

        private readonly IRepositoryAsync<File> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        private readonly IContactService _contactService;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;

        public FileService(IRepositoryAsync<File> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService,
            IContactService contactService, IAccountService accountService, IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
            _contactService = contactService;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<File>> FindAsyncByStatus(string fileType, string status, string orgId)
        {
            var orgIdGuid = orgId.ToSysGuid();
            if (status == Data.Helper.FileStatus.Queued.ToString() || status == Data.Helper.FileStatus.Review.ToString())
            {
                var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.FileType == fileType & p.Status == status)
                  .OrderBy(q => q.OrderBy(c => c.CreatedDate).ThenBy(c => c.AccountId))
                  .SelectAsync();

                return enumerable;
            }
            else if (status == Data.Helper.FileStatus.Submitted.ToString() || status == Data.Helper.FileStatus.Delivered.ToString())
            {
                var startDateTime = DateTime.UtcNow.AddMonths(-2);
                var endDateTime = DateTime.UtcNow;
                var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status))
                      .OrderBy(q => q.OrderBy(c => c.SubmittedDate).ThenBy(c => c.AccountId))
                      .SelectAsync();

                return enumerable;
            }
            else
            {
                var startDateTime = DateTime.UtcNow.AddMonths(-2);
                var endDateTime = DateTime.UtcNow;

                var enumerable = await _repository.Query(p => p.Active & p.FileType == fileType & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status))
                    .OrderBy(q => q.OrderBy(c => c.CreatedDate).ThenBy(c => c.AccountId))
                    .SelectAsync();

                return enumerable;
            }

        }

        public IEnumerable<File> FindByStatus(string fileType, string status, string orgId)
        {
            var orgIdGuid = orgId.ToSysGuid();
            if (status == Data.Helper.FileStatus.Queued.ToString() || status == Data.Helper.FileStatus.Review.ToString())
            {
                var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.FileType == fileType & p.Status == status)
                  .OrderBy(q => q.OrderBy(c => c.CreatedDate).ThenBy(c => c.AccountId))
                  .Select();

                return enumerable;
            }
            else if (status == Data.Helper.FileStatus.Submitted.ToString() || status == Data.Helper.FileStatus.Delivered.ToString())
            {
                var startDateTime = DateTime.UtcNow.AddMonths(-2);
                var endDateTime = DateTime.UtcNow;
                var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status))
                      .OrderBy(q => q.OrderBy(c => c.SubmittedDate).ThenBy(c => c.AccountId))
                      .Select();

                return enumerable;
            }
            else
            {
                var startDateTime = DateTime.UtcNow.AddMonths(-2);
                var endDateTime = DateTime.UtcNow;

                var enumerable = _repository.Query(p => p.Active & p.FileType == fileType & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status))
                    .OrderBy(q => q.OrderBy(c => c.CreatedDate).ThenBy(c => c.AccountId))
                    .Select();

                return enumerable;
            }
        }

        public IEnumerable<File> FindByUserAndStatus(string fileType, string status, string userId)
        {
            if (status == Data.Helper.FileStatus.Queued.ToString() || status == Data.Helper.FileStatus.Review.ToString())
            {
                var enumerable = _repository.Query(p => p.Active & p.FileType == fileType & p.Status == status & p.ProcessorId == userId)
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Select();

                return enumerable;
            }
            else if (status == Data.Helper.FileStatus.Delivered.ToString())
            {
                var startDateTime = DateTime.UtcNow.AddDays(-1);
                var endDateTime = DateTime.UtcNow;

                var enumerable = _repository.Query(p => p.Active & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status) & p.ProcessorId == userId)
                    .OrderBy(q => q.OrderByDescending(c => c.SubmittedDate).ThenBy(c => c.AccountId))
                    .Select();

                return enumerable;
            }
            else
            {
                var startDateTime = DateTime.UtcNow.AddMonths(-1);
                var endDateTime = DateTime.UtcNow;

                var enumerable = _repository.Query(p => p.Active & p.FileType == fileType & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status) & p.ProcessorId == userId)
                    .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
                    .Select();

                return enumerable;
            }
        }

        public bool AssignFileToUser(string userId, out string message)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
            {
                message = "userId is empty";
                return false;
            }
            var user = _aspNetUserService.Find(userId);

        //This is GoTo label to start search all over again if the fileis assigned to someone else during transaction
        SearchFile:
            //First check if user already hasve a assigned file
            var assignedFiles = FindByUserAndStatus("CV", Lifetrons.Erp.Data.Helper.FileStatus.Assigned.ToString(), userId);
            if (assignedFiles != null)
            {
                if (assignedFiles.Count() > 0)
                {
                    message = "Pending file/s found.";
                    return false;
                }
            }

            //Assign job
            var queuedFiles = FindByStatus("CV", Lifetrons.Erp.Data.Helper.FileStatus.Queued.ToString(), user.OrgId.ToString());
            Data.File assignedFile = null;

            if (queuedFiles != null)
            {
                if (queuedFiles.Count() >= 1)
                {
                    //Assign the first file by default
                    assignedFile = queuedFiles.FirstOrDefault();
                    //1. Check if PreferredOwnerId exists for OriginSenderMail contact
                    //2. Check if out of all "Queued" jobs,  any jobs exists with current userId == contact.PreferredOwnerId
                    //3. if the current job is with no contact.PreferredOwnerId and current userId has some "Queued" jobs then ? 
                    foreach (var file in queuedFiles)
                    {
                        //Check for files which has current logged in user as prefferred, if any then assign it to user.
                        var contacts = _contactService.FindByEmail(file.OriginSenderEmail);
                        var contact = contacts.FirstOrDefault();
                        if (contact != null)
                        {
                            if (contact.PreferredOwnerId == userId)
                            {
                                assignedFile = file;
                                break;
                            }
                        }
                    }
                }
            }

            //Update database with assigned job
            if (assignedFile != null)
            {
                var dbFile = _repository.Find(assignedFile.Id);
                if (dbFile.Status == Data.Helper.FileStatus.Queued.ToString())
                {
                    assignedFile.Status = Data.Helper.FileStatus.Assigned.ToString();
                    assignedFile.ProcessorId = userId;
                    assignedFile.ProcessorEmail = user.AuthenticatedEmail;
                    _repository.Update(assignedFile);
                    _unitOfWork.SaveChanges();
                    message = "New file/s assigned.";
                    return true;
                }
                else
                {
                    goto SearchFile;
                }
            }

            message = "No Files in Queue.";
            return false;
        }

        public async Task<IEnumerable<File>> FindAsyncByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            Guid accountIdGuid = accountId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.AccountId == accountIdGuid & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.SubmittedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<File> FindByAccount(string fileType, string accountId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            Guid accountIdGuid = accountId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.AccountId == accountIdGuid & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.SubmittedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Select();

            return enumerable;
        }

        public async Task<IEnumerable<File>> FindAsyncByAccount(string accountId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            Guid accountIdGuid = accountId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.AccountId == accountIdGuid & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.SubmittedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<File> FindByAccount(string accountId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            Guid accountIdGuid = accountId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.AccountId == accountIdGuid & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.SubmittedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .Select();

            return enumerable;
        }

        public IEnumerable<File> FindDeliveriesByProcessor(string fileType, string processorId, DateTime startDateTime, DateTime endDateTime)
        {

            var enumerable = _repository.Query(p => p.Active & p.ProcessorId == processorId & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == Data.Helper.FileStatus.Delivered.ToString()))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .Select();

            return enumerable;
        }

        public IEnumerable<File> FindDeliveriesByDate(string fileType, DateTime startDateTime, DateTime endDateTime, string orgId)
        {
            var orgIdGuid = orgId.ToSysGuid();
            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.FileType == fileType & (p.SubmittedDate >= startDateTime && p.SubmittedDate <= endDateTime && p.Status == Data.Helper.FileStatus.Delivered.ToString()))
              .OrderBy(q => q.OrderByDescending(c => c.SubmittedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .Select();

            return enumerable;
        }

        public async Task<IEnumerable<File>> FindAsyncByProcessor(string fileType, string processorId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            var enumerable = await _repository.Query(p => p.Active & p.ProcessorId == processorId & p.FileType == fileType & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<File> FindByProcessor(string fileType, string processorId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            var enumerable = _repository.Query(p => p.Active & p.ProcessorId == processorId & p.FileType == fileType & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .Select();

            return enumerable;
        }

        public async Task<IEnumerable<File>> FindAsyncByProcessor(string processorId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            var enumerable = await _repository.Query(p => p.Active & p.ProcessorId == processorId & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .SelectAsync();

            return enumerable;
        }

        public IEnumerable<File> FindByProcessor(string processorId, DateTime startDateTime, DateTime endDateTime, string status)
        {
            var enumerable = _repository.Query(p => p.Active & p.ProcessorId == processorId & (p.CreatedDate >= startDateTime && p.CreatedDate <= endDateTime && p.Status == status))
              .OrderBy(q => q.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.AccountId))
              .Include(p => p.Contact)
              .Include(p => p.Account)
              .Include(p => p.AspNetUserProcessor)
              .Include(p => p.Organization)
              .Select();

            return enumerable;
        }

        public File FindRecentFile(string fileType, string orgId)
        {
            var orgIdGuid = orgId.ToSysGuid();
            var lastFile = _repository.Query(p => p.Active & p.FileType == fileType & p.OrgId == orgIdGuid)
              .OrderBy(q => q.OrderByDescending(c => c.FileSentDate))
              .Select().FirstOrDefault();

            return lastFile;
        }

        public File Find(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var File = _repository.Find(id.ToSysGuid());
            if (File == null) return null;

            return File;
        }

        public async Task<File> FindAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var File = await _repository.FindAsync(id.ToSysGuid());
            if (File == null) return null;

            return File;
        }

        public File Create(File param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            _repository.Insert(param);
            return param;

        }

        public void Update(File param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            //Update
            _repository.Update(param);

        }

        public void Delete(string id)
        {
            //Do the validation here

            //Do business logic here

            _repository.Delete(id.ToSysGuid());
        }

        public IEnumerable<File> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var records = _repository
                .Query(q => !string.IsNullOrEmpty(q.FileType) & q.OrgId.Equals(orgIdGuid))
                .OrderBy(q => q
                    .OrderBy(c => c.FileType)
                    .ThenBy(c => c.Tags))
                .SelectPage(pageNumber, pageSize, out totalRecords);

            return records;

        }

        public void Dispose()
        {
        }

    }
}
