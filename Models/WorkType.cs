using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class WorkType
{
    public int TypeId { get; set; }

    public string НаименованиеРаботы { get; set; } = null!;

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
