using System;
using System.Collections.Generic;

namespace KASPDispetcher.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public virtual ICollection<Master> Masters { get; set; } = new List<Master>();
}
