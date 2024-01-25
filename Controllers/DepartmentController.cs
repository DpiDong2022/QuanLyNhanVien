using System;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.Contracts.Repositories;
using BaiTap_phan3.Models;
using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Filters;

namespace BaiTap_phan3.Controllers
{
     [MyActionFilter]
    public class DepartmentController : Controller
    {
        private readonly IGenericRepository<PhongBan> _repositoryPhongBan;
        public DepartmentController(IGenericRepository<PhongBan> genericRepository)
        {
            _repositoryPhongBan =  genericRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
           var result = await _repositoryPhongBan.GetAll() as IEnumerable<PhongBan>;
                return Json(result);
        }
    }
}