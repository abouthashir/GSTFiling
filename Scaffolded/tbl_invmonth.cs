using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_invmonth
{
    [Key]
    public int cmid { get; set; }

    public int? mid { get; set; }

    public int? yid { get; set; }

    public int? ctypid { get; set; }

    public int? cstmrid { get; set; }

    public int? userid { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? stid { get; set; }

    public bool? pymnt { get; set; }

    public bool? aprv { get; set; }

    public bool? cnslt { get; set; }

    public int? invtype { get; set; }

    public bool? invstatus { get; set; }

    public string? pymntbill { get; set; }
}
