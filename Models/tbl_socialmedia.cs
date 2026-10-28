using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_socialmedia
{
    [Key]
    public int socialid { get; set; }

    [StringLength(100)]
    public string? sname { get; set; }

    public int? priority { get; set; }

    public bool? actv { get; set; }

    [StringLength(100)]
    public string? linkname { get; set; }
}
