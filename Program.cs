using Microsoft.Extensions.DependencyInjection.Extensions;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.Models;
using BaiTap_phan3.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(
    option => {option.JsonSerializerOptions.PropertyNamingPolicy=null;});

// Use HttpContext
builder.Services.AddSingleton(typeof(DapperContext<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddSingleton(typeof(NhanVienRepository));
builder.Services.AddControllers();

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
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
