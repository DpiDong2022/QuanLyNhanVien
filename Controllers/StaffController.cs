using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Models;
using BaiTap_phan3.Response;
using BaiTap_phan3.Interfaces;
using BaiTap_phan3.DTO;

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
            await _nhanVienService.Xoa(id);
        }

        [HttpPost]
        public async Task<IActionResult> Report([FromBody] NhanVien[] nhanViens)
        {
            var result = await _nhanVienService.ToExcel(nhanViens);
            return Json(result);
        }

        [HttpGet]
        public FileResult Download(string filePath, string fileName){
            
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
