using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Models;
using BaiTap_phan3.Filters;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography.Pkcs;
using OfficeOpenXml;
using MessageApp2.DATA;

namespace BaiTap_phan3.Controllers
{
    [MyActionFilter]
    public class HomeController : Controller
    {
        private IHostEnvironment _hostEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;
        public HomeController(IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, IConfiguration configuration)
        {
            _hostEnvironment = hostingEnvironment;
            _logger = logger;
            _configuration = configuration;
            _appDbContext = new AppDbContext(_configuration);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
            LoginModel adminLogin = new LoginModel();
            _configuration.GetSection("AdminAccount").Bind(adminLogin);
            if (adminLogin.AccountName == loginModel.AccountName &&
                adminLogin.Password == loginModel.Password)
            {
                ViewBag.log = "Log out";
                return RedirectToAction("Index", "User");
            }

            User? user = _appDbContext.Users.FirstOrDefault(user => user.AccountName == loginModel.AccountName
                                                            && user.PassWord == loginModel.Password);
            if (user != null)
            {
                ViewBag.name = user.AccountName; 
                ViewBag.log = "Log out";
                ViewBag.AccountList = _appDbContext.Users.ToList();
                return View("Index");
            }
            else
            {
                ModelState.AddModelError("AccountName", "Tên và mật khẩu sai");
                return View(loginModel);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(ResponseError responseError)
        {
            responseError.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError(message: responseError.ToString());
            ViewBag.Message = ReasonPhrases.GetReasonPhrase(responseError.Code);
            return View(responseError);
        }
    }

}