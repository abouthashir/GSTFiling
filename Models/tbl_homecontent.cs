using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_homecontent
{
    [Key]
    public int midid { get; set; }

    public string? title1 { get; set; }

    public string? descr1 { get; set; }

    public string? title2 { get; set; }

    public string? descr2 { get; set; }

    [StringLength(50)]
    public string? img2 { get; set; }

    public string? title3 { get; set; }

    public string? descr3 { get; set; }

    [StringLength(50)]
    public string? img3 { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public bool? actv { get; set; }
}
