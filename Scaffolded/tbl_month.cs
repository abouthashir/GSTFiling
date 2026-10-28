using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_month
{
    [Key]
    public int mid { get; set; }

    [StringLength(50)]
    public string? month { get; set; }
}
