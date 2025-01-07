using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class СonstructionSite
{
    public int ObjectId { get; set; }

    public string? ObjectName { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
