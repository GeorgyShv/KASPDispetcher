using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASPDispetcher.Models;

public partial class Master
{
    [Key]
    public string UserId { get; set; }

    public int? PositionId { get; set; }

    public virtual Position? Position { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}
