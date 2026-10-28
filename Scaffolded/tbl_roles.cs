using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_roles
{
    [Key]
    public int roleid { get; set; }

    [StringLength(50)]
    public string? rolename { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public bool? actv { get; set; }

    public int? userid { get; set; }
}
