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

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortColumn { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool SortDescending { get; set; }

    public List<ReportViewModel> Reports { get; set; }

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

    public List<string> StatusOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        ConstructionSites = await _context.�onstructionSites.ToListAsync();

        StatusOptions = await _context.ReportStates
            .Select(s => s.StateName)
            .ToListAsync();

        var query = _context.Reports
            .Include(r => r.ReportStateJournals)
            .ThenInclude(j => j.State)
            .Include(r => r.Object)
            .AsQueryable();

        // ��������� ����������
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            query = query.Where(r =>
                r.��������������.ToString().Contains(SearchQuery) ||
                (r.Note != null && r.Note.Contains(SearchQuery)));
        }

        if (SelectedObjectId.HasValue)
        {
            query = query.Where(r => r.ObjectId == SelectedObjectId.Value);
        }

        if (!string.IsNullOrEmpty(DateFrom) && DateTime.TryParse(DateFrom, out var dateFrom))
        {
            query = query.Where(r => r.Data >= dateFrom);
        }

        if (!string.IsNullOrEmpty(DateTo) && DateTime.TryParse(DateTo, out var dateTo))
        {
            query = query.Where(r => r.Data <= dateTo);
        }

        if (Statuses != null && Statuses.Any())
        {
            query = query.Where(r => r.ReportStateJournals.Any(j =>
                j.State != null && Statuses.Contains(j.State.StateName)));
        }

        // ��������� ����������
        query = SortColumn switch
        {
            "��������������" => SortDescending
                ? query.OrderByDescending(r => r.��������������)
                : query.OrderBy(r => r.��������������),
            "����" => SortDescending
                ? query.OrderByDescending(r => r.Data)
                : query.OrderBy(r => r.Data),
            _ => query
        };

        // ��������� ������ � ����
        var reports = await query.ToListAsync();

        // ����������� ������ ��� �����������
        var users = await _context.Users.ToDictionaryAsync(u => u.Id, u => u.LastName);
        Reports = reports.Select(r => new ReportViewModel
        {
            ReportId = r.ReportId,
            �������������� = r.��������������,
            Data = r.Data,
            Note = r.Note,
            ObjectName = r.Object?.ObjectName ?? "��� ������",
            UserName = users.ContainsKey(r.UserId) ? users[r.UserId] : "��� ������",
            Status = r.ReportStateJournals
                .OrderByDescending(j => j.StateDate)
                .FirstOrDefault()?.State?.StateName ?? "��� ������",
            DepartmentId = r.DepartmentId
        }).ToList();
    }

    public async Task<IActionResult> OnPostAsync(DateTime Date, string DocumentNumber, string? Notes)
    {
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(DateTime Date, string DocumentNumber, int ObjectId, string? Notes)
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
                DepartmentId = user.DepartmentId,
                ObjectId = ObjectId
            };

            _context.Reports.Add(newReport);

            var newStateJournal = new ReportStateJournal
            {
                ReportId = newReportId,
                StateId = 4, // ������ "�� �����"
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

    public async Task<IActionResult> OnPostSubmitForApprovalAsync(int ReportId)
    {
        // ���������, ���������� �� �����
        var report = await _context.Reports
            .Include(r => r.ReportStateJournals)
            .FirstOrDefaultAsync(r => r.ReportId == ReportId);

        if (report == null)
        {
            return NotFound();
        }

        // ���������, ���������� �� ������ "��������" (StateId = 2)
        var approvalState = await _context.ReportStates.FirstOrDefaultAsync(s => s.StateId == 2);
        if (approvalState == null)
        {
            return NotFound("������ '��������' �� ������.");
        }

        // ���������, ���������� �� ��� ������ � ������� ��� ������� �������
        var existingState = report.ReportStateJournals
            .FirstOrDefault(j => j.StateId == approvalState.StateId);

        if (existingState == null)
        {
            // ��������� ����� ������ � ������ ��������
            var stateJournal = new ReportStateJournal
            {
                ReportId = ReportId,
                StateId = approvalState.StateId,
                StateDate = DateTime.Now
            };
            _context.ReportStateJournals.Add(stateJournal);

            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Reports");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int ReportId)
    {
        // ����� ����� �� ID
        var report = await _context.Reports
            .Include(r => r.ReportStateJournals)
            .FirstOrDefaultAsync(r => r.ReportId == ReportId);

        if (report == null)
        {
            return NotFound();
        }

        // ������� ��������� ������ �� ������� ���������
        _context.ReportStateJournals.RemoveRange(report.ReportStateJournals);

        // ������� �����
        _context.Reports.Remove(report);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Reports");
    }

    // ����� ��� ��������� ������ ReportId
    private async Task<int> GenerateReportIdAsync()
    {
        int maxReportId = await _context.Reports.MaxAsync(r => (int?)r.ReportId) ?? 0;
        System.Console.WriteLine("������������ Id:" + maxReportId);
        return maxReportId + 1;
    }
}
