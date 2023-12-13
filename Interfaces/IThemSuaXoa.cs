using System;
using BaiTap_phan3.Response;
using BaiTap_phan3.Models;

namespace BaiTap_phan3.Interfaces{

    public interface IThemSuaXoa{
        ResponseMvc Them(NhanVien nhanVien);
        ResponseMvc Sua(NhanVien nhanVien);
        ResponseMvc Xoa(string MaNhanVien);
    }
}