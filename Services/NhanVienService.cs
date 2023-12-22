using System;
using BaiTap_phan3.Interfaces;
using BaiTap_phan3.Response;
using BaiTap_phan3.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using Npgsql;
using System.Net.Http.Headers;
using Dapper;
using System.Threading.Tasks.Dataflow;
using System.Diagnostics;
using BaiTap_phan3.DBContext;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BaiTap_phan3.DTO;
using Microsoft.Extensions.WebEncoders;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Runtime.CompilerServices;
using OfficeOpenXml;
using System.ComponentModel;

namespace BaiTap_phan3.Services
{

    public class NhanVienService : INhanVienService
    {

        //private readonly IHttpContextAccessor _contextAccessor;
        //private readonly IConfiguration _configuration;
        public static List<NhanVien> NhanViens;
        private readonly DapperContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NhanVienService(DapperContext dapperContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = dapperContext;
            NhanViens = GetList().Result.ToList();
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<NhanVien>> GetList()
        {
            string query = "SELECT \"Id\", \"HoVaTen\", \"NgaySinh\", \"DienThoai\"," +
            "\"ChucVu\", \"PhongBan_Id\" as \"PhongBanId\" FROM \"NhanVien\"";
            using (var connection = _context.CreateConnection())
            {
                IEnumerable<NhanVien> nhanViens = await connection.QueryAsync<NhanVien>(query);
                return nhanViens;
            }
        }

        public async Task<ResponseMvc> Sua(int id, NhanVienDto nhanVien)
        {
            if (!IsValid(nhanVien.HoVaTen, nhanVien.NgaySinh, id))
            {
                return new ResponseMvc() { Message = "Họ tên và ngày sinh của nhân viên bị trùng" };
            }

            string query = """
                UPDATE "NhanVien"
                SET "HoVaTen" = @hoVaTen, "NgaySinh" = @ngaySinh, 
                            "DienThoai" = @dienThoai, "ChucVu" = @chucVu, "PhongBan_Id" = @phongBan_Id
                WHERE "Id" = @id
            """;

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id, DbType.Int32);
                    parameters.Add("hoVaTen", nhanVien.HoVaTen, DbType.String);
                    parameters.Add("ngaySinh", nhanVien.NgaySinh, DbType.Date);
                    parameters.Add("dienThoai", nhanVien.DienThoai, DbType.String);
                    parameters.Add("chucVu", nhanVien.ChucVu, DbType.String);
                    parameters.Add("phongBan_Id", nhanVien.PhongBanId, DbType.Int16);

                    int rowAffected = await connection.ExecuteAsync(query, parameters);

                    if (rowAffected > 0)
                    {
                        return new ResponseMvc() { Success = true };
                    }
                    else
                    {
                        return new ResponseMvc() { Message = "Id sai hoặc nhân viên không tồn tại" };
                    }
                }
                catch (Exception ex)
                {

                    return new ResponseMvc() { Message = ex.Message };
                }
            }

        }

        public async Task<ResponseMvc> Them(NhanVienDto nhanVien)
        {
            if (!IsValid(nhanVien.HoVaTen, nhanVien.NgaySinh))
            {
                throw new ArgumentException("Nhân viên đã tồn tại", nameof(nhanVien));
            }

            string query = "INSERT INTO \"NhanVien\"(\"HoVaTen\", \"NgaySinh\", \"DienThoai\", \"ChucVu\")" +
                                " VALUES(@HoVaTen, @NgaySinh, @DienThoai, @ChucVu)" +
                                " RETURNING \"Id\"";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    int id = await connection.QuerySingleAsync<int>(query, nhanVien);
                    if (id != 0)
                    {
                        return new ResponseMvc() { Success = true, data = id };
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }

                return new ResponseMvc() { Message = "Something wrong" };
            }
        }

        public async Task<ResponseMvc> Xoa(int id)
        {
            string query = "DELETE FROM \"NhanVien\" WHERE \"Id\" = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return new ResponseMvc() { Success = true };
            }
        }

        public async Task<ResponseMvc> ToExcel(NhanVien[] nhanViens)
        {
            string virtualPath = "\\Templates\\NhanVien.xlsx";
            string destinationpath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates\\NhanVienBaoCao.xlsx");
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, virtualPath.TrimStart('\\'));

            if (File.Exists(fullPath))
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                FileInfo infor = new FileInfo(fullPath);
                ExcelPackage excel = new ExcelPackage(infor);
                ExcelWorksheet sheet = excel.Workbook.Worksheets[0];

                int rowIndex = 3;
                int insertIndex = 8;
                
                for (int i = 0; i < nhanViens.Length; i++)
                {
                    if(rowIndex == insertIndex){
                        sheet.InsertRow(insertIndex, 1);
                        insertIndex++;
                        // copy style
                        sheet.Cells[3, 1, 3, 7].Copy(sheet.Cells[rowIndex, 1, rowIndex, 7]);
                    }
                    sheet.Cells[rowIndex, 1].Value = i+1;
                    sheet.Cells[rowIndex, 2].Value = nhanViens[i].Id;
                    sheet.Cells[rowIndex, 3].Value = nhanViens[i].HoVaTen;
                    sheet.Cells[rowIndex, 4].Value = nhanViens[i].NgaySinh.ToString("dd-MM-yyyy");
                    sheet.Cells[rowIndex, 5].Value = nhanViens[i].DienThoai;
                    sheet.Cells[rowIndex, 6].Value = nhanViens[i].ChucVu;
                    sheet.Cells[rowIndex, 7].Value = nhanViens[i].PhongBan.TenPhongBan;

                    rowIndex++;
                }
                await excel.SaveAsAsync(destinationpath);
                return new ResponseMvc() { Success = true, data = destinationpath};
            }
            else
            {
                return new ResponseMvc() { Message = "File khong ton tai" };
            }
        }

        private bool IsValid(string hoTen, DateTime ngaySinh, int id = -1)
        {
            hoTen = hoTen.ToLower();
            if (NhanViens.Any(c => c.HoVaTen.ToLower() == hoTen && c.NgaySinh == ngaySinh && c.Id != id))
            {
                return false;
            }
            return true;
        }
    }
}
