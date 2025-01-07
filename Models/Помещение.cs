using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class Помещение
{
    public int DepartmentId2 { get; set; }

    public int ObjectId { get; set; }

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
