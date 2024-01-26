using System.Diagnostics;
using System.Threading.Tasks;
using BaiTap_phan3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaiTap_phan3.Filters
{
    public class MyActionFilter : ActionFilterAttribute, IExceptionFilter
    {
        private string? _controllerName = "";
        private string? _actionName = "";
        public override void  OnActionExecuting(ActionExecutingContext context)
        {
            _controllerName = context.ActionDescriptor.RouteValues.ContainsKey("controller") ?
                            context.ActionDescriptor.RouteValues["controller"] : "CONTROLLER NOT AVAILABLE";
            _actionName = context.ActionDescriptor.RouteValues.ContainsKey("action") ?
                            context.ActionDescriptor.RouteValues["action"] : "ACTION NOT AVAILABLE";
        }

        public void OnException(ExceptionContext context)
        {
            ResponseError responseError = new ResponseError
            {
                Code = 500,
                Status = "Internal server error",
                Message = context.Exception.InnerException==null?context.Exception.Message:context.Exception.InnerException.Message,
                ControllerName = _controllerName,
                ActionName = _actionName,
                Time = DateTime.Now
            };
            //context.Result = new RedirectToActionResult("Error", "Home", responseError);
            context.Result = new JsonResult(responseError);
        }
    }
}