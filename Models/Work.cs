using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class Work
{
    public int WorkId { get; set; }

    public int ReportId { get; set; }

    public int? TypeId { get; set; }

    public int UserId { get; set; }

    public DateTime Attribute31 { get; set; }

    public DateTime ПериодКонец { get; set; }

    public int ЗанятоЧелПлан { get; set; }

    public virtual Report Report { get; set; } = null!;

    public virtual WorkType? Type { get; set; }

    public virtual ICollection<Помещение> DepartmentId2s { get; set; } = new List<Помещение>();
}
