using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_invoicebill
{
    [Key]
    public int bid { get; set; }

    public int? ascnsltid { get; set; }

    public int? cstmrid { get; set; }

    public int? cmid { get; set; }

    [Unicode(false)]
    public string? invoice { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? mid { get; set; }

    public int? yid { get; set; }
}
