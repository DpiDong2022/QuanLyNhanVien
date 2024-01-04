

using System.ComponentModel;
using System.Data.SqlClient;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.DTO;
using BaiTap_phan3.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BaiTap_phan3.Repository{

    public class NhanVienRepository : GenericRepository<NhanVien>
    {
        private readonly IHostEnvironment _hostEnvironment;
        public NhanVienRepository(DapperContext<NhanVien> context, IHostEnvironment hostEnvironment) : base(context)
        {
            _hostEnvironment = hostEnvironment;
        }

        public new string ToExcel(IEnumerable<NhanVienDto> nhanVienDtos){
            var nhanVienList = nhanVienDtos.ToArray();
            string templateVirtualPath = "wwwroot\\Templates\\NhanVien.xlsx";
            string fullPath = Path.Combine(_hostEnvironment.ContentRootPath, templateVirtualPath);

            ExcelPackage package = new ExcelPackage(fullPath);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var sheet = package.Workbook.Worksheets[0];


            if(nhanVienList.Count() > 15){
                var countAddedRow = nhanVienList.Count()-15;
                sheet.Cells[3,1,3,6].CopyStyles(sheet.Cells[18, 1, 18+countAddedRow, 6]);
            }

            for(int row = 0; row< nhanVienDtos.Count(); row++){
                sheet.Cells[row+3, 1].Value = row+1;
                sheet.Cells[row+3, 2].Value = nhanVienList[row].HoVaTen;
                sheet.Cells[row+3, 3].Value = nhanVienList[row].NgaySinh;
                sheet.Cells[row+3, 4].Value = nhanVienList[row].DienThoai;
                sheet.Cells[row+3, 5].Value = nhanVienList[row].ChucVu;
                sheet.Cells[row+3, 6].Value = nhanVienList[row].PhongBan.TenPhongBan;
            }

            // destination path
            string location = "wwwroot\\Templates\\NhanVienBaoCao.xlsx";
            string newFilePath = Path.Combine(_hostEnvironment.ContentRootPath, location);

            if(File.Exists(newFilePath)){
                File.Delete(newFilePath);
            }

            package.SaveAs(new FileInfo(newFilePath));
            return newFilePath;
        }
    }
}