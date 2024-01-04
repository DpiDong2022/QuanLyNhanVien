using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Models;
using BaiTap_phan3.Response;
using BaiTap_phan3.DTO;
using BaiTap_phan3.Repository;
using Newtonsoft.Json;

namespace BaiTap_phan3.Controllers
{

    public class StaffController : Controller
    {
        private readonly NhanVienRepository _repositoryNhanVien;
        private readonly IGenericRepository<PhongBan> _repositoryPhongBan;
        private ILogger<StaffController> _logger;
        public StaffController(NhanVienRepository repositoryNhanVien, IGenericRepository<PhongBan> repositoryPhongBan,ILogger<StaffController> logger)
        {
            _repositoryNhanVien = repositoryNhanVien;
            _repositoryPhongBan = repositoryPhongBan;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
             IEnumerable<NhanVienDto> nhanVienDtos =  await TimKiem(keySearch:"", pageSize: 10, pageNumber: 1, null);
            TempData["NhanVienInfor"] = JsonConvert.SerializeObject(nhanVienDtos);
            PageResult<NhanVienDto> result = new PageResult<NhanVienDto>(nhanVienDtos, 10, 1);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            try
            {
                var result = await _repositoryNhanVien.Insert(nhanVien);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, NhanVien nhanVien) //input value from asp-rout-maNhanVien
        {
            nhanVien.Id = id;
            bool result = await _repositoryNhanVien.Update(id, nhanVien);
            return Json(result);
        }

        [HttpPost]
        public async Task<object> Update(NhanVien nhanVien, int? id)
        {

            Thread.Sleep(500);
            if (id == null)
            {
                return await Create(nhanVien);
            }
            else
            {
                return await Edit((int)id, nhanVien);
            }

        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _repositoryNhanVien.Delete(id);
        }

        [HttpPost]
        public async Task<IActionResult> Report()
        {
            IEnumerable<NhanVienDto> ?nhanVienDtos = JsonConvert.DeserializeObject<IEnumerable<NhanVienDto>>(TempData["NhanVienInfor"].ToString());
            if(nhanVienDtos != null){
                var filePath = _repositoryNhanVien.ToExcel(nhanVienDtos);
                return Json(filePath);
            }
            return BadRequest();
        }

        [HttpGet]
        public FileResult Download(string filePath, string fileName){
            
            byte[] data = System.IO.File.ReadAllBytes(filePath);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keySearch, int pageSize, int pageNumber, int? phongBanId){
            if(phongBanId == 0) phongBanId = null;
            IEnumerable<NhanVienDto> nhanVienDtos =  await TimKiem(keySearch:keySearch, pageSize: pageSize, pageNumber: pageNumber, phongBanId);
            TempData["NhanVienInfor"] = JsonConvert.SerializeObject(nhanVienDtos);
            PageResult<NhanVienDto> result = new PageResult<NhanVienDto>(nhanVienDtos, pageSize, pageNumber);
            
            return PartialView("StaffBodyTable", result);
        }

        private async Task<IEnumerable<NhanVienDto>> TimKiem(string keySearch, int pageSize=10, int pageNumber=1, int? phongBanId = null){
            IEnumerable<NhanVien> nhanViens = await _repositoryNhanVien.GetAll();
            if(!string.IsNullOrEmpty(keySearch)){
                string[] keySplited = keySearch.Split(' '); 
                nhanViens = nhanViens.Where(nv => keySplited.Any(key => nv.HoVaTen.ToLower().Contains(key)));
            }

            if(phongBanId != null){
                nhanViens = nhanViens.Where(nv => nv.PhongBanId == phongBanId);
            }

            IEnumerable<PhongBan> phongBans = await _repositoryPhongBan.GetAll();
            IEnumerable<NhanVienDto> nhanVienDtos = nhanViens.Select(nv => new NhanVienDto(){
                Id = nv.Id,
                HoVaTen = nv.HoVaTen,
                NgaySinh = nv.NgaySinh,
                DienThoai = nv.DienThoai,
                ChucVu = nv.ChucVu,
                PhongBan = phongBans.First(pb => pb.Id == nv.PhongBanId)
            });

            return nhanVienDtos;
        }
    }
}
