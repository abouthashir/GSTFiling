using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_roleaccess
{
    [Key]
    public int id { get; set; }

    public int? roleid { get; set; }

    public int? pgid { get; set; }

    public int? userid { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }
}
