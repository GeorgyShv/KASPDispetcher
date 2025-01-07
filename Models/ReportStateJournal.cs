using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class ReportStateJournal
{
    public int ReportId { get; set; }

    public int StateId { get; set; }

    public DateTime StateDate { get; set; }

    public virtual Report Report { get; set; } = null!;

    public virtual ReportState State { get; set; } = null!;
}
