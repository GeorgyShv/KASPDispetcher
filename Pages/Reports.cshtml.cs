using KASPDispetcher.Models;
using KASPDispetcher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

public class ReportsModel : PageModel
{
    private readonly ApplicationContext _context;

    public ReportsModel(ApplicationContext context)
    {
        _context = context;
    }

    public List<Report> Reports { get; set; } = new();
    public List<�onstructionSite> ConstructionSites { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? SelectedObjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedUserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DateFrom { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DateTo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string[]? Statuses { get; set; }

    public async Task OnGetAsync()
    {
        ConstructionSites = await _context.�onstructionSites.ToListAsync();

        var query = _context.Reports
        .Include(r => r.Object)
        .Include(r => r.User)
        .Include(r => r.ReportStateJournals) // �������� ���������
        .ThenInclude(j => j.State) // �������� �������� ���������
        .AsQueryable();

        if (SelectedObjectId.HasValue)
        {
            query = query.Where(r => r.ObjectId == SelectedObjectId.Value);
        }

        if (!string.IsNullOrEmpty(SelectedUserId))
        {
            query = query.Where(r => r.UserId.Contains(SelectedUserId));
        }

        if (!string.IsNullOrEmpty(DateFrom) && DateTime.TryParse(DateFrom, out var dateFrom))
        {
            query = query.Where(r => r.Data >= dateFrom);
        }

        if (!string.IsNullOrEmpty(DateTo) && DateTime.TryParse(DateTo, out var dateTo))
        {
            query = query.Where(r => r.Data <= dateTo);
        }

        Reports = await query.ToListAsync();
    }
    public async Task<IActionResult> OnPostAsync(DateTime Date, string DocumentNumber, string? Notes)
    {
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(DateTime Date, string DocumentNumber, string? Notes)
    {
        try
        {
            if (!int.TryParse(DocumentNumber, out int parsedDocumentNumber))
            {
                ViewData["ErrorMessage"] = "����� ��������� ������ ���� ����� ������.";
                return Page(); // ���������� �������� � �������
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "������������ �� ������.";
                return Page();
            }

            int newReportId = await GenerateReportIdAsync();

            var newReport = new Report
            {
                ReportId = newReportId,
                Data = Date,
                �������������� = parsedDocumentNumber,
                Note = Notes,
                UserId = userId,
                DepartmentId = user.DepartmentId
            };

            _context.Reports.Add(newReport);

            var newStateJournal = new ReportStateJournal
            {
                ReportId = newReportId,
                StateId = 2, // "��������"
                StateDate = DateTime.Now
            };
            _context.ReportStateJournals.Add(newStateJournal);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Reports");
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = $"������: {ex.Message}";
            return Page();
        }
    }


    // ����� ��� ��������� ������ ReportId
    private async Task<int> GenerateReportIdAsync()
    {
        int maxReportId = await _context.Reports.MaxAsync(r => (int?)r.ReportId) ?? 0;
        System.Console.WriteLine("������������ Id:" + maxReportId);
        return maxReportId + 1;
    }
}
