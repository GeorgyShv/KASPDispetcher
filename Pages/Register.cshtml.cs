using KASPDispetcher;
using KASPDispetcher.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Для работы с базой данных
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context; // Используем объединённый контекст

    public RegisterModel(UserManager<User> userManager, ApplicationContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [BindProperty]
    public string Email { get; set; }
    [BindProperty]
    public string Password { get; set; }
    [BindProperty]
    public string ConfirmPassword { get; set; }
    [BindProperty]
    public int SelectedDepartmentId { get; set; } // Выбранное подразделение
    [BindProperty]
    public string FirstName { get; set; }
    [BindProperty]
    public string LastName { get; set; }
    [BindProperty]
    public string? MiddleName { get; set; }

    // Для выпадающего списка подразделений
    public List<Подразделение> Departments { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Загружаем список подразделений из объединённого контекста
        Departments = await _context.Подразделениеs.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают.";
            await OnGetAsync(); // Перезагрузить список подразделений
            return Page();
        }

        var user = new User
        {
            UserName = Email,
            Email = Email,
            DepartmentId = SelectedDepartmentId,
            FirstName = FirstName,
            LastName = LastName,
            MiddleName = MiddleName
        };

        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
        {
            return RedirectToPage("/Login");
        }

        ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
        await OnGetAsync(); // Перезагрузить список подразделений
        return Page();
    }
}
