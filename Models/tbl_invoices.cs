using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_invoices
{
    [Key]
    public int invid { get; set; }

    [StringLength(50)]
    public string? invnumber { get; set; }

    [StringLength(50)]
    public string? invdate { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? invamtdec { get; set; }

    [StringLength(50)]
    public string? invamt { get; set; }

    [StringLength(150)]
    public string? gstnum { get; set; }

    public int? txid { get; set; }

    public int? cmid { get; set; }

    public int? userid { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? amtgstout { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? sgst { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? cgst { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? cess { get; set; }

    [StringLength(150)]
    public string? customer { get; set; }
}
