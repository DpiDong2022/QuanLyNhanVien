using System;
using BaiTap_phan3.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaiTap_phan3.Controllers
{
    public class DepartmentController : Controller{
        private readonly IPhongBanService _phongBanService;
        public DepartmentController(IPhongBanService phongBanService){
                _phongBanService = phongBanService;
        }

        [HttpGet]
        public IActionResult Index(){
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetList(){
            var result = await _phongBanService.GetList();
            return Json(result);
        }
    }
}