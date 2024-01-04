using System.Collections.Generic;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.DTO;
using BaiTap_phan3.Models;

namespace BaiTap_phan3.Repository{

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DapperContext<T> _context;
        public GenericRepository(DapperContext<T> context)
        {
            _context = context;
        }

        public async Task<bool> Delete(object id)
        {
            return await _context.Delete(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.GetAll();
        }

        public  Task<T> GetById(object id)
        {
            throw new NotImplementedException();
        }

        public  async Task<object> Insert(T obj)
        {
            return await _context.Insert(obj);
        }

        public async Task<bool> Update(object id, T obj)
        {
            return await _context.Update(id, obj);
        }

        public string ToExcel(IEnumerable<NhanVienDto> nhanVienDtos){
            return "";
        }
    }
}