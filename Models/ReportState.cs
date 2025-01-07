using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class ReportState
{
    public int StateId { get; set; }

    public string StateName { get; set; } = null!;

    public virtual ICollection<ReportStateJournal> ReportStateJournals { get; set; } = new List<ReportStateJournal>();
}
