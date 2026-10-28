using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_Status
{
    [Key]
    public int stid { get; set; }

    [StringLength(50)]
    public string? status { get; set; }
}
