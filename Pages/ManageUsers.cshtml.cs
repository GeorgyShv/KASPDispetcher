using KASPDispetcher.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KASPDispetcher.Pages
{
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUsersModel(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserWithRoles> Users { get; set; }
        public List<string> AvailableRoles { get; set; }

        [BindProperty]
        public string StatusMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; }

        public string SortDirectionIcon => SortDescending ? "⬇️" : "⬆️";

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAssignRoleAsync(string UserId, string RoleName)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null || string.IsNullOrEmpty(RoleName))
            {
                StatusMessage = "Ошибка: пользователь или роль не найдены.";
                await LoadDataAsync();
                return Page();
            }

            if (!await _roleManager.RoleExistsAsync(RoleName))
            {
                StatusMessage = "Ошибка: роль не существует.";
                await LoadDataAsync();
                return Page();
            }

            var result = await _userManager.AddToRoleAsync(user, RoleName);
            StatusMessage = result.Succeeded
                ? $"Роль '{RoleName}' успешно назначена пользователю {user.Email}."
                : "Ошибка назначения роли.";

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveRoleAsync(string UserId, string RoleName)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null || string.IsNullOrEmpty(RoleName))
            {
                StatusMessage = "Ошибка: пользователь или роль не найдены.";
                await LoadDataAsync();
                return Page();
            }

            var result = await _userManager.RemoveFromRoleAsync(user, RoleName);
            StatusMessage = result.Succeeded
                ? $"Роль '{RoleName}' успешно удалена у пользователя {user.Email}."
                : "Ошибка удаления роли.";

            await LoadDataAsync();
            return Page();
        }

        private async Task LoadDataAsync()
        {
            var usersQuery = _userManager.Users.AsQueryable();

            // Поиск
            if (!string.IsNullOrEmpty(SearchEmail))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(SearchEmail));
            }

            // Загружаем пользователей
            var users = await usersQuery.ToListAsync();

            // Загружаем роли пользователей
            var userList = new List<UserWithRoles>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRoles
                {
                    Id = user.Id,
                    FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(), // Формирование ФИО
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            // Сортировка
            Users = SortColumn switch
            {
                "FullName" => SortDescending
                    ? userList.OrderByDescending(u => u.FullName).ToList()
                    : userList.OrderBy(u => u.FullName).ToList(),
                "Email" => SortDescending
                    ? userList.OrderByDescending(u => u.Email).ToList()
                    : userList.OrderBy(u => u.Email).ToList(),
                "Roles" => SortDescending
                    ? userList.OrderByDescending(u => u.Roles.Count).ToList()
                    : userList.OrderBy(u => u.Roles.Count).ToList(),
                _ => userList
            };

            // Список доступных ролей
            AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
        }

        public class UserWithRoles
        {
            public string Id { get; set; }
            public string FullName { get; set; } // Новое свойство
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }
    }
}
