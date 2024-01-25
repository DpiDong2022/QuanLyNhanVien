

using MessageApp2.DATA;
using BaiTap_phan3.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageApps2.Controllers{

    public class UserController: Controller{
        private readonly AppDbContext _appDbContext;

        public UserController()
        {
            _appDbContext = new AppDbContext();
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
            user.CreateDate = DateTime.Now;
            _appDbContext.Users.Add(user);
            _appDbContext.SaveChanges();
            return View("Index");
        }
    }
}