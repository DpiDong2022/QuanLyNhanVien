using Microsoft.Extensions.DependencyInjection.Extensions;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.Models;
using BaiTap_phan3.Contracts.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using BaiTap_phan3.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(
    option => {option.JsonSerializerOptions.PropertyNamingPolicy=null;});

// Use HttpContext
builder.Services.AddSingleton(typeof(DapperContext<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IStaffRepository<NhanVien>), typeof(StaffRepository));
builder.Services.AddControllers(cfg => cfg.Filters.Add(typeof(MyActionFilter)));

// use HttpContext

// for session
// builder.Services.AddDistributedMemoryCache();   
// builder.Services.AddSession(option => option.IdleTimeout = TimeSpan.FromMinutes(140));
// IdleTimeout sẽ khiến nội dung trong sesstion biến mất trong bao lâu,
// nếu load lại trang khi sesstion chưa bị xóa thì thười gian sẽ được tính lại
//for session

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Staff/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
    app.UseExceptionHandler(options => options.Run(async context =>{
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var ex = context.Features.Get<IExceptionHandlerFeature>();
        if(ex != null){
            await context.Response.WriteAsync("Internal server error: " +"\n" +ex.Error.Message);
        }
    }));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Index}/{id?}");

app.Run();
