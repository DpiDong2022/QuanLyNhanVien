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

    // sửa trả về lỗi modelstate.isvalue
    // tìm kiếm phân trang phía db, sử dụng index và fulltext
    [MyActionFilter]
    public class StaffController : Controller
    {
        private readonly IStaffRepository<NhanVien> _repositoryNhanVien;
        private readonly IGenericRepository<PhongBan> _repositoryPhongBan;
        private readonly IHostEnvironment _hostEnvironment;
        public StaffController(IHostEnvironment hostingEnvironment, IStaffRepository<NhanVien> repositoryNhanVien, IGenericRepository<PhongBan> repositoryPhongBan)
        {
            _repositoryNhanVien = repositoryNhanVien;
            _repositoryPhongBan = repositoryPhongBan;
            _hostEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                PageResult<NhanVien> pageResult = await _repositoryNhanVien.TimKiem("", -1, new Pagination { PageNumber = 1, PageSize = 8 });
                ViewBag.PageResult = pageResult;
                ViewBag.PhongBans = await _repositoryPhongBan.GetAll();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ResponseError { Message = ex.InnerException.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            ViewBag.PhongBans = await _repositoryPhongBan.GetAll();
            if (nhanVien.PhongBanId == 0)
            {
                ModelState.AddModelError("PhongBanId", "PHòng ban là bắt buộc");
            }
            if (!ModelState.IsValid)
            {
                return Json(new ResponseError()
                {
                    Status = "Bad request",
                    Code = 400,
                    Data = RenderRazorViewToString(this, "_createStaffModal", nhanVien)
                });
            }

            if (KiemTraNhanVienChung(nhanVien))
            {
                ModelState.AddModelError("HoVaTen", "Nhân viên đã tồn tại");
                ModelState.AddModelError("NgaySinh", "Ngày sinh bị trùng");
            }

            if (!ModelState.IsValid)
            {
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
                NhanVien nhanVienResult = await _repositoryNhanVien.Insert(nhanVien);
                nhanVienResult.PhongBan = await _repositoryPhongBan.GetById(nhanVienResult.PhongBanId);
                return Json(new ResponseError() { Data = nhanVienResult });
            }
        }

        private bool KiemTraNhanVienChung(NhanVien nhanVien)
        {
            List<NhanVien> nhanViens = _repositoryNhanVien.GetAll().Result;
            nhanVien.HoVaTen = nhanVien.HoVaTen.ToLower();
            int id = nhanVien.Id ?? 0;
            if (id == 0)
            {
                bool result = nhanViens.
                           Any(nv => nv.HoVaTen.ToLower() == nhanVien.HoVaTen &&
                               nv.NgaySinh == nhanVien.NgaySinh);
                return result;
            }
            else
            {
                return nhanViens.
                           Any(nv => nv.HoVaTen.ToLower() == nhanVien.HoVaTen &&
                               nv.NgaySinh == nhanVien.NgaySinh &&
                               nv.Id != id);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(NhanVien nhanVien)
        {
            ViewBag.PhongBans = await _repositoryPhongBan.GetAll();
            if (!ModelState.IsValid)
            {
                return Json(new ResponseError()
                {
                    Status = "Bad request",
                    Code = 400,
                    Message = "Yêu cầu không thể được thực thi do lỗi xác thực dữ liệu",
                    Data = RenderRazorViewToString(this, "_createStaffModal", nhanVien)
                });
            }

            if (KiemTraNhanVienChung(nhanVien))
            {
                ModelState.AddModelError("HoVaTen", "Nhân viên đã tồn tại");
                ModelState.AddModelError("NgaySinh", "Ngày sinh bị trùng");
            }

            if (!ModelState.IsValid)
            {
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
                NhanVien nhanVienResult = await _repositoryNhanVien.Update(nhanVien, nhanVien.Id);
                nhanVienResult.PhongBan = await _repositoryPhongBan.GetById(nhanVienResult.PhongBanId);
                return Json(new ResponseError() { Data = nhanVienResult });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _repositoryNhanVien.Delete(id);
            return Json(new ResponseBase() { Status = "OK", Code = 200 });
        }

        [HttpGet]
        public async Task<IActionResult> Report(string keySearch, int phongBanId)
        {

            IEnumerable<NhanVien>? nhanViens = _repositoryNhanVien.TimKiem(keySearch, phongBanId, new Pagination() ,false).Result.Data;
            var filePath = ToExcel(nhanViens);
            return Json(new ResponseBase{ Data = filePath});
        }

        [HttpGet]
        public IActionResult Download(string filePath, string fileName)
        {
            byte[] data = System.IO.File.ReadAllBytes(filePath);
            FileContentResult fileContentResult = File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            return fileContentResult;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keySearch, int phongBanId, Pagination pagination)
        {
            PageResult<NhanVien> pageResult = await _repositoryNhanVien.TimKiem(keySearch, phongBanId, pagination);
            ViewBag.PageResult = pageResult;
            return Json(new ResponseBase{ Data = RenderRazorViewToString(this, "_staffBodyTable") });
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
