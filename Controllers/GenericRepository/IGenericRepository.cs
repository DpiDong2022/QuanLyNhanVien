using System.Collections.Generic;

namespace BaiTap_phan3.Repository{

    public interface IGenericRepository<T> where T : class{

        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<object> Insert(T obj);
        Task<bool> Update(object id, T obj);
        Task<bool> Delete(object id);
    }
}