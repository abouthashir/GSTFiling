using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class tbl_taxrate
{
    [Key]
    public int txid { get; set; }

    [StringLength(50)]
    public string? txrate { get; set; }
}
