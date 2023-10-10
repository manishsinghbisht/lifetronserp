//using Lifetrons.Erp.Data;
//using Microsoft.Practices.Unity;
//using Repository;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Repository.Pattern.Ef6;
//using Repository.Pattern.Repositories;
//using Repository.Pattern.UnitOfWork;
//using Service.Pattern;
//using WebGrease.Css.Extensions;

//namespace Lifetrons.Erp.Service
//{
//    public class ProdPlanService : Service<ProdPlan>, IProdPlanService
//    {
//        //[Dependency]
//        //public Repository<ProdPlanDetail> LineItemRepositoryProdPlanDetail { get; set; }

//        [Dependency]
//        public Repository<Process> ProcessRepository { get; set; }

//        [Dependency]
//        public IUnitOfWork UnitOfWork { get; set; }

//        private readonly IRepositoryAsync<ProdPlan> _repository;
//        private readonly IAspNetUserService _aspNetUserService;

//        public ProdPlanService(IRepositoryAsync<ProdPlan> repository, IAspNetUserService aspNetUserService)
//            : base(repository)
//        {
//            _repository = repository;
//            _aspNetUserService = aspNetUserService;
//        }

//        public IEnumerable<ProdPlan> Select(string id, string userId, string orgId)
//        {
//            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

//            Guid orgIdGuid = orgId.ToSysGuid();
//            Guid quoteIdGuid = id.ToSysGuid();

//            var enumerable = _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.Id == quoteIdGuid)
//              .Include(p => p.AspNetUser)
//              .Include(p => p.AspNetUser1)
//              .Include(p => p.Organization)
//              .Include(p => p.Process)
//              .Select();

//            //Check user & org
//            var applicationUser = _aspNetUserService.Find(userId);
//            if (orgIdGuid == applicationUser.OrgId) return enumerable;

//            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//        }

//        public async Task<IEnumerable<ProdPlan>> SelectAsync(string userId, string orgId)
//        {
//            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

//            Guid orgIdGuid = orgId.ToSysGuid();

//            var enumerable = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid)
//              .Include(p => p.AspNetUser)
//              .Include(p => p.AspNetUser1)
//              .Include(p => p.Organization)
//              .Include(p => p.Process) 
//              .SelectAsync();

//            //Check user & org
//            var applicationUser = await _aspNetUserService.FindAsync(userId);
//            if (orgIdGuid == applicationUser.OrgId) return enumerable;

//            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//        }

//        public ProdPlan Find(string id, string userId, string orgId)
//        {
//            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

//            Guid orgIdGuid = orgId.ToSysGuid();
//            var record = _repository.Find(id.ToSysGuid());
//            if (record == null) return null;

//            //Check user & org
//            var applicationUser = _aspNetUserService.Find(userId);
//            if (record.OrgId == applicationUser.OrgId)
//            {
//                record.Process = ProcessRepository.Find(record.ProcessId);
//                return record;
//            }

//            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//        }

//        public async Task<ProdPlan> FindAsync(string id, string userId, string orgId)
//        {
//            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

//            var record = await _repository.FindAsync(id.ToSysGuid());
//            if (record == null) return null;

//            //Check user & org
//            var applicationUser = await _aspNetUserService.FindAsync(userId);
//            if (record.OrgId == applicationUser.OrgId)
//            {
//                record.Process = await ProcessRepository.FindAsync(record.ProcessId);
//                return record;
//            }

//            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//        }

//        public ProdPlan Create(ProdPlan param, string userId, string orgId)
//        {
//            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;
//            var applicationUser = _aspNetUserService.Find(userId);

//            //Business validation
//            /* Planning is done consolidated for all processes. 
//            So Assembly will be the start point and medium for planning to issue job for production. 
//            So this prodPlan.ProcessId wll be picked up in ProdPlanDetail.ProcessId as well*/
//            param.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();
//            Validate(param, applicationUser);

//            //Check user & org
//            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
//            {
//                _repository.Insert(param);
//                return param;
//            }

//            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//        }

//        private void Validate(ProdPlan param, AspNetUser applicationUser)
//        {
//            //Check start and end dates
//            if (param.StartDateTime >= param.EndDateTime) throw new ApplicationException("Start date time should be less than End date time");
            
//        }
//        public void Update(ProdPlan param, string userId, string orgId)
//        {
//            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
//                throw new ApplicationException("Parameters cannot be null or empty");
            
//            var applicationUser = _aspNetUserService.Find(userId);
            
//            //Business validation
//            /* Planning is done consolidated for all processes. 
//            So Assembly will be the start point and medium for planning to issue job for production. 
//            So this prodPlan.ProcessId wll be picked up in ProdPlanDetail.ProcessId as well*/
//            param.ProcessId = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();
//            Validate(param, applicationUser);
            
//            //Check user & org
//            if (param.OrgId == applicationUser.OrgId && applicationUser.Id == param.ModifiedBy)
//            {
//                //Update
//                _repository.Update(param);
//            }
//            else
//            {
//                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//            }
//        }

//        public new void Delete(ProdPlan model)
//        {
//            //// Begin transaction
//            UnitOfWork.BeginTransaction();
//            try
//            {
//                //IRepository<ProdPlanDetail> lineItemRepository = LineItemRepositoryProdPlanDetail;
                
//                //Delete details records
//                //var lineProducts = lineItemRepository.Query(p => p.ProdPlanId == model.Id).Select();
//                //lineProducts.ForEach(lineItemRepository.Delete);
                
//                //Delete master records 
//                _repository.Delete(model);

//                UnitOfWork.SaveChanges();

//                //// Commit Transaction
//                UnitOfWork.Commit();
//            }
//            catch (Exception exception)
//            {
//                //// Rollback transaction
//                UnitOfWork.Rollback();

//                throw;
//            }
//        }

//        public IEnumerable<ProdPlan> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
//        {
//            Guid orgIdGuid = orgId.ToSysGuid();

//            var applicationUser = _aspNetUserService.Find(userId);
//            if (applicationUser.OrgId == orgIdGuid)
//            {
//                var records = _repository
//                    .Query(q => !string.IsNullOrEmpty(q.ProcessId.ToString()) & q.OrgId.Equals(orgIdGuid))
//                    .OrderBy(q => q
//                        .OrderBy(c => c.ProcessId)
//                        .ThenBy(c => c.StartDateTime)
//                        .ThenBy(c => c.EndDateTime))
//                    .SelectPage(pageNumber, pageSize, out totalRecords);

//                return records;
//            }
//            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
//        }
//        public void Dispose()
//        {
//        }

//    }
//}
