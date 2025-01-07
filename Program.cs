using KASPDispetcher;
using KASPDispetcher.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрация контекста базы данных
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

// Регистрация Identity с кастомной моделью User
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationContext>();

// Добавление Razor Pages
builder.Services.AddRazorPages();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Убедитесь, что роль существует
    var roleName = "Admin";
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    // Найдите пользователя по email
    var user = await userManager.FindByEmailAsync("mr.gosha9@mail.ru");
    if (user != null && !await userManager.IsInRoleAsync(user, roleName))
    {
        // Назначьте роль
        await userManager.AddToRoleAsync(user, roleName);
        Console.WriteLine($"Роль {roleName} успешно назначена пользователю {user.Email}");
    }
    else
    {
        Console.WriteLine($"Пользователь не найден или уже имеет роль {roleName}");
    }
}

// Конфигурация HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Добавлено для поддержки аутентификации
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
