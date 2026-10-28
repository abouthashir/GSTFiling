using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_msg
{
    [Key]
    public int msgid { get; set; }

    public int? ascnsltid { get; set; }

    public string? message { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? amt { get; set; }

    public int? cstmrid { get; set; }

    public bool? actv { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? mid { get; set; }

    public int? yid { get; set; }
}
