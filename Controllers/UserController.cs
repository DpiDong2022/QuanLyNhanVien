

using MessageApp2.DATA;
using BaiTap_phan3.Models;
using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Filters;

namespace MessageApps2.Controllers{

    [MyActionFilter]
    public class UserController: Controller{
        private readonly AppDbContext _appDbContext;
        public UserController(IConfiguration configuration)
        {

            _appDbContext = new AppDbContext(configuration);
        }

        public IActionResult Index(){
            ViewBag.Users = _appDbContext.Users;
            return View();
        }

        [HttpGet]
        public IActionResult Create(){
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user){
            if(!ModelState.IsValid){
                return View(user);
            }
            string newAccountName = user.AccountName.ToLower();
            if(_appDbContext.Users.ToList().FirstOrDefault(user => user.AccountName.ToLower()==newAccountName)!=null){
                ModelState.AddModelError("AccountName","Tên tài khoản đã tồn tại");
            }
             if(!ModelState.IsValid){
                return View(user);
            }
            user.CreatedDate = DateTime.Now;
            _appDbContext.Users.Add(user);
            _appDbContext.SaveChanges();
            return RedirectToAction("Index","User");
        }
    }
}