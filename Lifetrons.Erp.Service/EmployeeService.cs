using System;
using Lifetrons.Erp.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class EmployeeService : Service<Employee>, IEmployeeService
    {
        private readonly IRepositoryAsync<Employee> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IOrganizationService _organizationService;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public EmployeeService(IRepositoryAsync<Employee> repository, IAspNetUserService aspNetUserService, IOrganizationService organizationService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
            _organizationService = organizationService;
        }

        public IEnumerable<Employee> Select(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }
            var employees = _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
               .Include(p => p.AspNetUser)
               .Include(p => p.AspNetUser1)
               .Include(p => p.Organization)
               .Include(p => p.Department)
               .Include(p => p.Contact)
               .Select();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (orgIdGuid == applicationUser.OrgId) return employees;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Employee>> SelectAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var employees = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.Organization)
                .Include(p => p.Department)
                .Include(p => p.Contact)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return employees;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Employee>> SelectAsync(string userId, string orgId, List<Guid> containsIds)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var employees = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & containsIds.Contains(p.Id))
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.Organization)
                .Include(p => p.Department)
                .Include(p => p.Contact)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return employees;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<IEnumerable<Employee>> SelectAsyncDepartmentEmployees(string departmentId, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            Guid departmentIdGuid = departmentId.ToSysGuid();
            if (orgIdGuid == Guid.Empty || departmentIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var employees = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.DepartmentId == departmentIdGuid)
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.Organization)
                .Include(p => p.Department)
                .Include(p => p.Contact)
                .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId) return employees;

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public async Task<Employee> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var employee = await _repository.FindAsync(id.ToSysGuid());
            if (employee == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (employee.OrgId == applicationUser.OrgId)
            {
                employee.Organization = await _organizationService.FindAsync(orgId, userId, orgId);
                return employee;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Employee Find(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();
            if (orgIdGuid == Guid.Empty)
            {
                throw new ApplicationException("Empty key recieved", new Exception("Empty key recieved."));
            }

            var employee = _repository.Find(id.ToSysGuid());
            if (employee == null) return null;

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (employee.OrgId == applicationUser.OrgId)
            {
                employee.Organization = _organizationService.Find(orgId, userId, orgId);
                return employee;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public Employee Create(Employee param, string userId, string orgId)
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
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(Employee param, string userId, string orgId)
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

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        public IEnumerable<Employee> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
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
                    .Query(q => !string.IsNullOrEmpty(q.Contact.LastName))
                    .OrderBy(q => q
                        .OrderBy(c => c.Contact.FirstName)
                        .ThenBy(c => c.Contact.LastName))
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
