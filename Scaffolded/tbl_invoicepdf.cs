using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_invoicepdf
{
    [Key]
    public int invexid { get; set; }

    [StringLength(400)]
    public string? invexcel { get; set; }

    public int? cmid { get; set; }

    public int? userid { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }
}
