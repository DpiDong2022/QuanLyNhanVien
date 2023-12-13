using Microsoft.Extensions.DependencyInjection.Extensions;
using BaiTap_phan3.Function;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use HttpContext
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// use HttpContext

// for session
builder.Services.AddDistributedMemoryCache();   
builder.Services.AddSession(option => option.IdleTimeout = TimeSpan.FromMinutes(140));
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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
