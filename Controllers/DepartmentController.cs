using System;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.Repository;
using BaiTap_phan3.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaiTap_phan3.Controllers
{
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
            try
            {
                var result = await _repositoryPhongBan.GetAll();
                return Json(result);
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
    }
}