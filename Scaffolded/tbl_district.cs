using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_district
{
    [Key]
    public int did { get; set; }

    [StringLength(200)]
    public string? dname { get; set; }

    public bool? actv { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? userid { get; set; }

    public int? priority { get; set; }

    [StringLength(500)]
    public string? img { get; set; }
}
