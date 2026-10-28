using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_test
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    public string? name { get; set; }
}
