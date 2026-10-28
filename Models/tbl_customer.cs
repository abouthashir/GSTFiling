using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models;

public partial class tbl_customer
{
    [Key]
    public int cstmrid { get; set; }

    [StringLength(150)]
    public string? cstmrname { get; set; }

    public string? cstmraddress { get; set; }

    [StringLength(150)]
    public string? email { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? mob { get; set; }

    [StringLength(150)]
    public string? gstcode { get; set; }

    public int? priority { get; set; }

    [StringLength(50)]
    public string? updte { get; set; }

    public int? userid { get; set; }

    [StringLength(150)]
    public string? shop { get; set; }

    [StringLength(150)]
    public string? gstuname { get; set; }

    [StringLength(150)]
    public string? gstpwd { get; set; }

    public int? cnsltid { get; set; }
}
