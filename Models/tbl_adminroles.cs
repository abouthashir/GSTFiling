using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_adminroles
{
    [Key]
    public int roleid { get; set; }

    [StringLength(50)]
    public string? type { get; set; }
}
