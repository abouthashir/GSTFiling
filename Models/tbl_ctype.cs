using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_ctype
{
    [Key]
    public int ctypid { get; set; }

    [StringLength(50)]
    public string? ctype { get; set; }

    public int? priority { get; set; }
}
