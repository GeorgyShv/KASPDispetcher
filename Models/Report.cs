using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? DepartmentId { get; set; }

    public int? ObjectId { get; set; }

    public string? UserId { get; set; }

    public int НомерДокумента { get; set; }

    public DateTime Data { get; set; }

    public string? Note { get; set; }

    public virtual Подразделение? Department { get; set; }

    public virtual СonstructionSite? Object { get; set; }

    public virtual ICollection<ReportStateJournal> ReportStateJournals { get; set; } = new List<ReportStateJournal>();

    public virtual Master? User { get; set; }

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
