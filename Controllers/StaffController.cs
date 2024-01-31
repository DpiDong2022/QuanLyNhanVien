using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Models;
using BaiTap_phan3.Contracts.Repositories;
using OfficeOpenXml;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Drawing.Printing;
using BaiTap_phan3.Filters;
using Microsoft.AspNetCore.OutputCaching;
using System.Text.RegularExpressions;


namespace BaiTap_phan3.Controllers
{
    // kiem tra trung V
    // phongban minvalue V
    // tim kiem co su uu tien V
    // select khong select all (*) V
    // them sua tra ve (id,truefalse) V
    
    // thêm nút bỏ lọc và áp dụng lọc V

    // sửa trả về lỗi modelstate.isvalue V
    // tìm kiếm phân trang phía db, sử dụng index và fulltext V
    [MyActionFilter]
    public class StaffController : Controller
    {
        private readonly IStaffRepository<NhanVien> _repositoryNhanVien;
        private readonly IGenericRepository<PhongBan> _repositoryPhongBan;
        private readonly IGenericRepository<ChucVu> _repositoryChucVu;
        private readonly IHostEnvironment _hostEnvironment;
        private static List<PhongBan> _phongBanList;
        private readonly List<ChucVu> _chucVuList;
        public StaffController(IHostEnvironment hostingEnvironment, IStaffRepository<NhanVien> repositoryNhanVien, IGenericRepository<PhongBan> repositoryPhongBan, IGenericRepository<ChucVu> repositoryChucVu)
        {
            _repositoryNhanVien = repositoryNhanVien;
            _repositoryPhongBan = repositoryPhongBan;
            _repositoryChucVu = repositoryChucVu;
            _hostEnvironment = hostingEnvironment;
            _phongBanList = repositoryPhongBan.GetAll().Result;
            _chucVuList = repositoryChucVu.GetAll().Result;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                PageResult<NhanVien> pageResult = await _repositoryNhanVien.TimKiem("", -1, -1, new Pagination { PageNumber = 1, PageSize = 8 });
                ViewBag.PageResult = pageResult;
                ViewBag.PhongBans = await _repositoryPhongBan.GetAll();
                ViewBag.ChucVus = await _repositoryChucVu.GetAll();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ResponseError { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (nhanVien.PhongBanId == 0)
            {
                ModelState.AddModelError("PhongBanId", "PHòng ban là bắt buộc");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.PhongBans = _phongBanList;
                ViewBag.ChucVus = _chucVuList;
                return Json(new ResponseError()
                {
                    Status = "Bad request",
                    Code = 400,
                    Data = RenderRazorViewToString(this, "_createStaffModal", nhanVien)
                });
            }

            if (await _repositoryNhanVien.KiemTraTrung(nhanVien))
            {
                ModelState.AddModelError("HoVaTen", "Nhân viên đã tồn tại");
                ModelState.AddModelError("NgaySinh", "Ngày sinh bị trùng");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PhongBans = _phongBanList;
                ViewBag.ChucVus = _chucVuList;
                return Json(new ResponseError()
                {
                    Status = "Bad request",
                    Code = 400,
                    Message = "Yêu cầu không thể được thực thi do lỗi xác thực dữ liệu",
                    Data = RenderRazorViewToString(this, "_createStaffModal", nhanVien)
                });
            }
            else
            {
                int nhanVienMoi_id = await _repositoryNhanVien.Insert(nhanVien);
                return Json(new ResponseError() { Data = nhanVienMoi_id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(NhanVien nhanVien)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PhongBans = _phongBanList;
                ViewBag.ChucVus = _chucVuList;
                return Json(new ResponseError()
                {
                    Status = "Bad request",
                    Code = 400,
                    Message = "Yêu cầu không thể được thực thi do lỗi xác thực dữ liệu",
                    Data = RenderRazorViewToString(this, "_createStaffModal", nhanVien)
                });
            }

            if (await _repositoryNhanVien.KiemTraTrung(nhanVien))
            {
                ModelState.AddModelError("HoVaTen", "Nhân viên đã tồn tại");
                ModelState.AddModelError("NgaySinh", "Ngày sinh bị trùng");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PhongBans = _phongBanList;
                ViewBag.ChucVus = _chucVuList;
                return Json(new ResponseError()
                {
                    Status = "Bad request",
                    Code = 400,
                    Message = "Yêu cầu không thể được thực thi do lỗi xác thực dữ liệu",
                    Data = RenderRazorViewToString(this, "_createStaffModal", nhanVien)
                });

            }
            else
            {
                bool isSuccess = await _repositoryNhanVien.Update(nhanVien, nhanVien.Id);
                return Json(new ResponseError() { Data = isSuccess });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _repositoryNhanVien.Delete(id);
            return Json(new ResponseBase() { Status = "OK", Code = 200 });
        }

        [HttpGet]
        public async Task<IActionResult> Report(string keySearch, int phongBanId, int chucVu)
        {

            IEnumerable<NhanVien>? nhanViens = _repositoryNhanVien.TimKiem(keySearch, phongBanId, chucVu, new Pagination(), false).Result.Data;
            var filePath = ToExcel(nhanViens);
            return Json(new ResponseBase { Data = filePath });
        }

        [HttpGet]
        public IActionResult Download(string filePath, string fileName)
        {
            byte[] data = System.IO.File.ReadAllBytes(filePath);
            FileContentResult fileContentResult = File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            return fileContentResult;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keySearch, int phongBanId, int chucVuId, Pagination pagination)
        {
            PageResult<NhanVien> pageResult = await _repositoryNhanVien.TimKiem(keySearch, phongBanId, chucVuId, pagination);
            ViewBag.PageResult = pageResult;
            return Json(new ResponseBase { Data = RenderRazorViewToString(this, "_staffBodyTable") });
        }

        public string ToExcel(IEnumerable<NhanVien> nhanViens)
        {
            var nhanVienList = nhanViens.ToArray();
            string templateVirtualPath = "wwwroot\\Templates\\NhanVien.xlsx";
            string fullPath = Path.Combine(_hostEnvironment.ContentRootPath, templateVirtualPath);

            ExcelPackage package = new ExcelPackage(fullPath);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var sheet = package.Workbook.Worksheets[0];


            if (nhanVienList.Count() > 15)
            {
                var countAddedRow = nhanVienList.Count() - 15;
                sheet.Cells[3, 1, 3, 6].CopyStyles(sheet.Cells[18, 1, 18 + countAddedRow, 6]);
            }

            for (int row = 0; row < nhanViens.Count(); row++)
            {
                sheet.Cells[row + 3, 1].Value = row + 1;
                sheet.Cells[row + 3, 2].Value = nhanVienList[row].HoVaTen;
                sheet.Cells[row + 3, 3].Value = nhanVienList[row].NgaySinh.ToString("dd-MM-yyyy");
                sheet.Cells[row + 3, 4].Value = nhanVienList[row].DienThoai;
                sheet.Cells[row + 3, 5].Value = nhanVienList[row].ChucVu;
                sheet.Cells[row + 3, 6].Value = nhanVienList[row].PhongBan.TenPhongBan;
            }

            // destination path
            string location = "wwwroot\\Templates\\NhanVienBaoCao.xlsx";
            string newFilePath = Path.Combine(_hostEnvironment.ContentRootPath, location);

            if (System.IO.File.Exists(newFilePath))
            {
                System.IO.File.Delete(newFilePath);
            }

            package.SaveAs(new FileInfo(newFilePath));
            return newFilePath;
        }

        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
