using Microsoft.AspNetCore.Identity;

public class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("Master"))
        {
            await roleManager.CreateAsync(new IdentityRole("Master"));
        }
    }

    public static async Task RemoveRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                Console.WriteLine($"Роль '{roleName}' успешно удалена.");
            }
            else
            {
                Console.WriteLine($"Ошибка при удалении роли '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"Роль '{roleName}' не найдена.");
        }
    }
}
