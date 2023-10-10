using Lifetrons.Erp.Data;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Service
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> SelectAsync(string userId, string orgId, int month, int year);
        Task<IEnumerable<Attendance>> SelectAsync(string userId, string orgId, string employeeId, int month, int year);
        Attendance Find(string id, string userId, string orgId);
        Task<Attendance> FindAsync(string id, string userId, string orgId);
        Attendance Create(Attendance param, string userId, string orgId);
        void Update(Attendance param, string userId, string orgId);
        IEnumerable<Attendance> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }

    public class AttendanceService : Service<Attendance>, IAttendanceService
    {
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IRepositoryAsync<Attendance> _repository;

        public AttendanceService(IRepositoryAsync<Attendance> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<Attendance>> SelectAsync(string userId, string orgId, int month, int year)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.InDateTime.Month == month & p.InDateTime.Year== year)
                    .OrderBy(q => q.OrderBy(c => c.Employee.Name).ThenBy(c => c.InDateTime))
                    .Include(p => p.AspNetUser)
                    .Include(p => p.AspNetUser1)
                    .Include(p => p.Employee)
                    .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Attendance>> SelectAsync(string userId, string orgId, string employeeId, int month, int year)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid employeeIdGuid = employeeId.ToSysGuid();
            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.EmployeeId == employeeIdGuid & p.InDateTime.Month == month & p.InDateTime.Year == year)
                    .OrderBy(q => q.OrderBy(c => c.Employee.Name).ThenBy(c => c.InDateTime))
                    .Include(p => p.AspNetUser)
                    .Include(p => p.AspNetUser1)
                    .Include(p => p.Employee)
                    .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return enumerable;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Attendance Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var attendance = _repository.Find(id.ToSysGuid());
            if (attendance == null) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (attendance.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return attendance;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<Attendance> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            var task = await _repository.FindAsync(id.ToSysGuid());
            if (task == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Attendance Create(Attendance param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();
            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business validation
                Validate(param, applicationUser);

                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        private void Validate(Attendance param, AspNetUser applicationUser)
        {
            if (param == null || string.IsNullOrEmpty(param.CreatedBy) || string.IsNullOrEmpty(param.ModifiedBy))
                throw new ApplicationException("Parameters cannot be null or empty");

            if (param.InDateTime == param.OutDateTime)
                throw new ApplicationException("In time and out time cannot be same.");

            if (param.InDateTime.Date != param.OutDateTime.Date)
                throw new ApplicationException("Dates of In time and out time should be same.");


            if (param.InDateTime.Hour > param.OutDateTime.Hour)
                throw new ApplicationException("Out time cannot be less than in time.");

        }

        public void Update(Attendance param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
            {
                //Business validation
                Validate(param, applicationUser);

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

        public IEnumerable<Attendance> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();
            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.Employee.Name)
                        .ThenBy(c => c.InDateTime))
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
