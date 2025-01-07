using KASPDispetcher.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KASPDispetcher.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationContext _context;

        public List<User> Users { get; private set; } = new();

        public IndexModel(ILogger<IndexModel> logger,
            ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            Users = _context.Users.AsNoTracking().ToList();
        }
    }
}
