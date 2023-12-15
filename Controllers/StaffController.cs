
using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Services;
using BaiTap_phan3.Models;
using Newtonsoft.Json;
using BaiTap_phan3.Response;

namespace BaiTap_phan3.Controllers
{

    public class StaffController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NhanVienService _nhanVienService;
        public StaffController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _nhanVienService = new NhanVienService(_httpContextAccessor, configuration);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(NhanVien nhanVien)
        {
            var result = _nhanVienService.Them(nhanVien);
            return Json(result);
        }

        [HttpGet]
        public IActionResult Edit(NhanVien nhanVien) //input value from asp-rout-maNhanVien
        {
            ResponseMvc result = _nhanVienService.Sua(nhanVien);
            return Json(result);

        }

        [HttpPost]
        public IActionResult Update(NhanVien nhanVien)
        {
            if(string.IsNullOrEmpty(nhanVien.MaNhanVien)){

                return Create(nhanVien);
            }else{
                 return Edit(nhanVien);
            }

        }

        [HttpGet]
        public void Delete(string MaNhanVien)
        {
            var result = _nhanVienService.Xoa(MaNhanVien);

        }

        [HttpGet]
        public IActionResult Report()
        {
            return Content("Đang xây dựng");

        }

        [HttpGet]
        public IActionResult List(){
            Task<IEnumerable<NhanVien>> result = _nhanVienService.List();
            return Json(result.Result);
        }
    }
}