using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_aboutus
{
    [Key]
    public int abtid { get; set; }

    [StringLength(500)]
    public string? abttitle { get; set; }

    public string? abtdescr { get; set; }

    [StringLength(150)]
    public string? bnrimg { get; set; }

    [StringLength(150)]
    public string? abtimg { get; set; }

    [StringLength(50)]
    public string? dte { get; set; }

    public bool? actv { get; set; }
}
