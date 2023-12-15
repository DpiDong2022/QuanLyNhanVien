using System;
using BaiTap_phan3.Interfaces;
using BaiTap_phan3.Response;
using BaiTap_phan3.Models;
using BaiTap_phan3.Function;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using Npgsql;
using System.Net.Http.Headers;
using Dapper;

namespace BaiTap_phan3.Services
{

    public class NhanVienService : IThemSuaXoa
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly DBServices<NhanVien> _dBServices;
        public NhanVienService(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _dBServices = new DBServices<NhanVien>(_configuration);
        }

        public ResponseMvc Sua(NhanVien nhanVienMoi)
        {

            ResponseMvc response = new ResponseMvc();
            Dictionary<string, NhanVien> nhanVienDictionary = CenterTool.GetDanhSachNhanVien();

            foreach (var item in nhanVienDictionary)
            {
                if (item.Key != nhanVienMoi.MaNhanVien &&
                    item.Value.HoVaTen == nhanVienMoi.HoVaTen &&
                    item.Value.NgayThangNamSinh == nhanVienMoi.NgayThangNamSinh)
                {

                    response.Success = false;
                    response.Message = $"Nhân viên này đã tồn tại và có mã là {item.Key}";
                    return response;
                }
            }
            response.Success = true;
            response.data = nhanVienMoi.MaNhanVien.ToUpper();

            CenterTool.ContextAccessor.HttpContext.Session.SetString(
                nhanVienMoi.MaNhanVien, JsonConvert.SerializeObject(nhanVienMoi));
            return response;

        }

        public ResponseMvc Them(NhanVien nhanVienMoi)
        {
            Dictionary<string, NhanVien> nhanVienDictionary = CenterTool.GetDanhSachNhanVien();
            var response = new ResponseMvc(){};
            foreach (var nhanVien in nhanVienDictionary)
            {
                if (nhanVienMoi.HoVaTen == nhanVien.Value.HoVaTen && nhanVienMoi.NgayThangNamSinh == nhanVien.Value.NgayThangNamSinh)
                {

                    response.Success = false;
                    response.Message = $"Nhân viên này đã tồn tại và có mã là {nhanVien.Key}";
                    return response;
                }
            }
            response.Success = true;
            nhanVienMoi.MaNhanVien = CenterTool.GenerateMaNv();
            response.data = nhanVienMoi.MaNhanVien.ToUpper();

            CenterTool.ContextAccessor.HttpContext.Session.SetString(
            nhanVienMoi.MaNhanVien, JsonConvert.SerializeObject(nhanVienMoi));
            return response;
        }

        public ResponseMvc Xoa(string keyNV)
        {
            CenterTool.ContextAccessor.HttpContext.Session.Remove(keyNV);
            return new ResponseMvc() { Success = true, Message = "Xóa thành công" };

        }

        public async Task<IEnumerable<NhanVien>> List()
        {
            IEnumerable<NhanVien> nhanViens = _dBServices.GetTable("NhanVien").Result;
            return nhanViens;
        }
    }
}
