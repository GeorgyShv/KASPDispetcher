namespace KASPDispetcher.Models
{
    public class ReportViewModel
    {
        public int ReportId { get; set; }
        public int НомерДокумента { get; set; }
        public DateTime Data { get; set; }
        public string? Note { get; set; }
        public string? ObjectName { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; }
        public int? DepartmentId { get; set; }
    }
}
