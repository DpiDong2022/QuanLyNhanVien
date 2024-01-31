
using BaiTap_phan3.Models;
using BaiTap_phan3.DBContext;

namespace BaiTap_phan3.Contracts.Repositories{

      public interface IGenericRepository<T>{
        Task<List<T>> GetAll();
        Task<int> Insert(T obj);
        Task<bool> Update(T obj, object id);
        Task<bool> Delete(object id);
        Task<T> GetById(object id);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DapperContext<T> _dbContext;

        public GenericRepository(DapperContext<T> dapperContext)
        {
            _dbContext = dapperContext;
        }

        public async Task<bool> Delete(object id)
        {
            return await _dbContext.Delete(id);
        }

        public async Task<List<T>> GetAll()
        {
            return await _dbContext.GetAll();
        }

        public async Task<T> GetById(object id)
        {
            return await _dbContext.GetById(id);
        }

        public async Task<int> Insert(T obj)
        {
            return await _dbContext.Insert(obj);
        }

        public async Task<bool> Update(T obj, object id)
        {
            return await _dbContext.Update(id, obj);
        }
    }
}