using System;
using Lifetrons.Erp.Data;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public class JoiningRequestService : Service<JoiningRequest>, IJoiningRequestService
    {
        private readonly IRepositoryAsync<JoiningRequest> _repository;
        //private readonly IUnitOfWorkForService _unitOfWork;

        public JoiningRequestService(IRepositoryAsync<JoiningRequest> repository) : base(repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<JoiningRequest>> GetAsync(string userId, string orgId )
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            return _repository.Query(p => p.SubmittedTo == userId & p.Status == "PENDING")
                .Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.AspNetUser2)
                .SelectAsync();
            //return _repository.Query().GetAsync();
        }

        public Task<JoiningRequest> FindAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            return _repository.FindAsync(id.ToSysGuid());
        }

        public Task<IEnumerable<JoiningRequest>> FindAsyncEnumerable(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            return _repository.Query(q => q.Id.ToString() == id).Include(p => p.AspNetUser)
                .Include(p => p.AspNetUser1)
                .Include(p => p.AspNetUser2)
                .SelectAsync();
        }

        public JoiningRequest Create(JoiningRequest param)
        {
            if (param == null )
                throw new ApplicationException("Parameters cannot be null or empty");
            _repository.Insert(param);
            return param;
        }

        public void Update(JoiningRequest param)
        {
            //Do the validation here

            //Do business logic here

            //Update
            _repository.Update(param);
        }

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        public IEnumerable<JoiningRequest> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var records = _repository
                .Query(q => !string.IsNullOrEmpty(q.RequestComments))
                .OrderBy(q => q
                    .OrderBy(c => c.RequestDate)
                    .ThenBy(c => c.RequestComments))
                .SelectPage(pageNumber, pageSize, out totalRecords);

            return records;
        }
        public void Dispose()
        {
        }

    }
}
