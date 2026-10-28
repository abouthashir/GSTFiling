using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_homebanner
{
    [Key]
    public int bnrid { get; set; }

    [StringLength(500)]
    public string? bnrtitle { get; set; }

    public string? bnrdescr { get; set; }

    public int? priority { get; set; }

    public bool? actv { get; set; }

    [StringLength(250)]
    public string? bnrimg { get; set; }

    [StringLength(500)]
    public string? url { get; set; }
}
