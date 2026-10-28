using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

[Keyless]
public partial class tbl_update
{
    public int? id { get; set; }

    public int? claimid { get; set; }

    [StringLength(500)]
    public string? CustomerName { get; set; }

    [StringLength(500)]
    public string? IMEI { get; set; }

    [StringLength(500)]
    public string? ProductName { get; set; }

    [StringLength(500)]
    public string? ShopName { get; set; }

    [StringLength(50)]
    public string? QRCode { get; set; }

    [StringLength(500)]
    public string? PurchasedOn { get; set; }
}
