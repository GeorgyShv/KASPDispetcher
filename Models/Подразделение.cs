using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class Подразделение
{
    public int DepartmentId { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
