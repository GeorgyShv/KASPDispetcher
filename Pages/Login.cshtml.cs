using KASPDispetcher.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    [BindProperty]
    public string Email { get; set; }
    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public LoginModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Email, Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        ErrorMessage = "Неверный логин или пароль.";
        return Page();
    }
}