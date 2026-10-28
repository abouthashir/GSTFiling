using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_adminuser
{
    [Key]
    public int userid { get; set; }

    [StringLength(500)]
    public string? uname { get; set; }

    [StringLength(50)]
    public string? pwd { get; set; }

    [StringLength(150)]
    public string? email { get; set; }

    public int? roleid { get; set; }

    public bool? actv { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? cid { get; set; }
}
