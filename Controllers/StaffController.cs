using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Services;
using BaiTap_phan3.Models;
using Newtonsoft.Json;
using BaiTap_phan3.Response;
using BaiTap_phan3.Interfaces;
using BaiTap_phan3.DTO;
using System.IO;
using System.Net;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BaiTap_phan3.Controllers
{

    public class StaffController : Controller
    {
        private readonly INhanVienService _nhanVienService;
        private ILogger<StaffController> _logger;
        public StaffController(INhanVienService nhanVienService, ILogger<StaffController> logger)
        {
            _nhanVienService = nhanVienService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVienDto nhanVien)
        {
            try
            {
                var result = await _nhanVienService.Them(nhanVien);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, NhanVienDto nhanVien) //input value from asp-rout-maNhanVien
        {
            ResponseMvc result = await _nhanVienService.Sua(id, nhanVien);
            if (result.Success)
            {
                return Json(result);
            }
            return Json(result);

        }

        [HttpPost]
        public async Task<IActionResult> Update(NhanVienDto nhanVien, int id = -1)
        {
            if (id == -1)
            {
                return await Create(nhanVien);
            }
            else
            {
                return await Edit(id, nhanVien);
            }

        }

        [HttpGet]
        public async Task Delete(int id)
        {
            var result = await _nhanVienService.Xoa(id);

        }

        [HttpPost]
        public async Task<ContentResult> Report([FromBody] NhanVien[] nhanViens)
        {
            var filePath = await _nhanVienService.ToExcel(nhanViens);
            return Content((string)filePath.data);
        }

        [HttpGet]
        public IActionResult Download(string filePath, string fileName){
            byte[] data = System.IO.File.ReadAllBytes(filePath);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<NhanVien> result = await _nhanVienService.GetList();
            return Json(result);
        }
    }
}

//         public void Do()
//         {
//             using (var session = OpenSession())
//             {
//                 var condition = GenerateConditionThietHaiLinhVuc(model);

//                 var suKienThienTai = await session.FindAsync<SuKienThienTai>(stm => stm.Where($"{condition}"));

//                 IEnumerable<ThietHai> thietHai = new List<ThietHai>();
//                 if (suKienThienTai.Count() > 0)
//                 {

//                     thietHai = await session.QueryAsync<ThietHai>(@$"
// select
// 	sum(th.so_luong) as so_luong,
// 	sum(th.giatri_thiethai) as giatri_thiethai, 
// 	th.ma_chitieu 
// from
// 	thiet_hai th
// 	where th.thientai_id in ({string.Join(",", suKienThienTai.Select(x => x.id))})
// 	group by th.ma_chitieu;");

//                 }


//                 IEnumerable<Area> areas = new List<Area>();
//                 if (suKienThienTai.Count() > 0)
//                 {
//                     areas = await session.FindAsync<Area>(stm => stm
//                                            .Include<ThienTaiKhuVuc>(x => x.LeftOuterJoin())
//                                            .Where($"{Sql.TableAndColumn<ThienTaiKhuVuc>(nameof(ThienTaiKhuVuc.thientai_id))} in ({string.Join(",", suKienThienTai.Select(x => x.id))}) "));
//                 }



//                 ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//                 var pathFileExcel = System.IO.Path.Combine(GlobalConfiguration.WebRootPath, "public\\document\\bieumau_thiethai_chung.xlsx");
//                 using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(pathFileExcel)))
//                 {
//                     var sheetThietHaiChung = excelPackage.Workbook.Worksheets[0];
//                     ExcelRange cell;

//                     var row = 5;
//                     cell = sheetThietHaiChung.Cells[row++, 7];
//                     cell.Value = string.Join(", ", areas.Where(w => w.area_type == 1).OrderBy(o => o.area_name).Select(x => x.area_name));

//                     cell = sheetThietHaiChung.Cells[row++, 7];
//                     cell.Value = string.Join(", ", areas.Where(w => w.area_type == 2).OrderBy(o => o.area_name).Select(x => x.area_name));

//                     cell = sheetThietHaiChung.Cells[row++, 7];
//                     cell.Value = string.Join(", ", areas.Where(w => w.area_type == 3).OrderBy(o => o.area_name).Select(x => x.area_name));

//                     cell = sheetThietHaiChung.Cells[row++, 7];
//                     cell.Value = string.Join(", ", suKienThienTai.OrderBy(o => o.ten_sukien).Select(x => x.ten_sukien));

//                     int totalRows = sheetThietHaiChung.Dimension.End.Row;
//                     int totalCols = sheetThietHaiChung.Dimension.End.Column;
//                     for (int rowIndex = 16; rowIndex < totalRows; rowIndex++)
//                     {
//                         var ma_chitieu = EPPlusCellHelper.GetCellStringValue(sheetThietHaiChung.Cells[rowIndex, 2].Value);
//                         var thiethai_theo_chitieu = thietHai.FirstOrDefault(x => x.ma_chitieu == ma_chitieu);
//                     }
//                 }
//             }
//         }