using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_centre
{
    [Key]
    public int cid { get; set; }

    [StringLength(200)]
    public string? cname { get; set; }

    public int? did { get; set; }

    public bool? actv { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? userid { get; set; }

    public int? priority { get; set; }

    [StringLength(150)]
    public string? contactname { get; set; }

    [StringLength(150)]
    public string? email { get; set; }

    [StringLength(50)]
    public string? mob1 { get; set; }

    [StringLength(50)]
    public string? mob2 { get; set; }

    public string? address { get; set; }
}
