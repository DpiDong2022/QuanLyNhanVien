using System;
using BaiTap_phan3.Response;
using BaiTap_phan3.Models;
using BaiTap_phan3.DTO;

namespace BaiTap_phan3.Interfaces{

    public interface IPhongBanService : IXoa<PhongBan>
    {
        Task<ResponseMvc> Them(PhongBan nhanVien);
        Task<ResponseMvc> Sua(int id, PhongBan nhanVien);
    }
}