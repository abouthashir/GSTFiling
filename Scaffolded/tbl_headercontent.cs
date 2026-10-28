using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_headercontent
{
    [Key]
    public int hid { get; set; }

    [StringLength(100)]
    public string? telephone { get; set; }

    [StringLength(100)]
    public string? mobile { get; set; }

    [StringLength(150)]
    public string? displayemailid { get; set; }

    [StringLength(150)]
    public string? mailemailid { get; set; }

    [StringLength(50)]
    public string? headdate { get; set; }

    [StringLength(500)]
    public string? address { get; set; }
}
