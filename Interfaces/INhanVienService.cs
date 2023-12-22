using System;
using BaiTap_phan3.Response;
using BaiTap_phan3.Models;
using BaiTap_phan3.DTO;

namespace BaiTap_phan3.Interfaces{

    public interface INhanVienService : IXoa<NhanVien>
    {
        Task<ResponseMvc> Them(NhanVienDto nhanVien);
        Task<ResponseMvc> Sua(int id, NhanVienDto nhanVien);
        Task<ResponseMvc> ToExcel(NhanVien[] nhanViens);
    }
}