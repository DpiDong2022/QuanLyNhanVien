using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BaiTap_phan3.Models;
using BaiTap_phan3.Filters;
using Microsoft.AspNetCore.WebUtilities;

namespace BaiTap_phan3.Controllers;

[MyActionFilter]
public class HomeController : Controller
{
    private IHostEnvironment _hostEnvironment;
    private readonly ILogger<HomeController> _logger;
    public HomeController(IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger)
    {
        _hostEnvironment = hostingEnvironment;
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
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
