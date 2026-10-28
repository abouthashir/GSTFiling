using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_invconsultant
{
    [Key]
    public int ascnsltid { get; set; }

    public int? did { get; set; }

    public int? cnsltid { get; set; }

    public int? cmid { get; set; }

    public bool? reply { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? userid { get; set; }

    public int? mid { get; set; }

    public int? yid { get; set; }

    public int? cstmrid { get; set; }
}
