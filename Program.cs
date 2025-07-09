using DACS2.Data;
using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DACS2.Models.Momo;
using DACS2.Services.Momo;
using DACS2.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================
// 1. Cấu hình dịch vụ
// =============================

//Momo API Payment
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();




// Đăng ký các repository (DI)
builder.Services.AddScoped<IChuDe, EFChuDe>();
builder.Services.AddScoped<IKhoaHoc, EFKhoaHoc>();
builder.Services.AddScoped<IBaiHoc, EFBaiHoc>();
builder.Services.AddScoped<IGiaoDich, EFGiaoDich>();
builder.Services.AddScoped<INguoiDung, EFNguoiDung>();
builder.Services.AddScoped<ICT_KhoaHoc_NguoiDung, EFCT_KhoaHoc_NguoiDung>();
builder.Services.AddScoped<ITestCase, EFTestCase>();


//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();


var app = builder.Build();

// =============================
// 2. Middleware
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =============================
// 3. Tạo Role và User mặc định
// =============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>(); // Get the ApplicationDbContext

    await CreateRolesAsync(roleManager, userManager, dbContext);
}

async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
{
    string[] roleNames = { "Admin", "NguoiDung" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Admin
    string adminEmail = "admin@example.com";
    string adminPassword = "Ad@123";

    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var adminUser = new ApplicationUser
        {
            FullName = "Admin",
            UserName = adminEmail,
            DiaChi = "Hà Nội",
            Tuoi = 18,
            GioiTinh = 1,
            NgaySinh = DateOnly.Parse("2004-04-08"),
            Email = adminEmail,
            PhoneNumber = "0932999901",
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            Console.WriteLine($"Lỗi tạo Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

    }

    // Người dùng thường
    string userEmail = "user@example.com";
    string userPassword = "Ad@123";

    if (await userManager.FindByEmailAsync(userEmail) is null)
    {
        var user = new ApplicationUser
        {
            FullName = "User",
            UserName = userEmail,
            DiaChi = "Hà Nội",
            Tuoi = 18,
            NgaySinh = DateOnly.Parse("2004-04-08"),
            GioiTinh = 1,
            Email = userEmail,
            PhoneNumber = "0932777702",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, userPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "NguoiDung");
        }
        else
        {
            Console.WriteLine($"Lỗi tạo User: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

     
    }

    await dbContext.SaveChangesAsync();
}

// =============================
// 4. Cấu hình endpoint
// =============================
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();
