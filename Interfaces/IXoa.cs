using System;
using BaiTap_phan3.Response;
using BaiTap_phan3.Models;

namespace BaiTap_phan3.Interfaces{

    public interface IXoa<T>{
        Task<ResponseMvc> Xoa(int id);
        Task<IEnumerable<T>> GetList();
    }
}