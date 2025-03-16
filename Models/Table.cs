using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("tables")]
public partial class Table
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("contain")]
    public int? Contain { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Table")]
    public virtual ICollection<OrderBill> OrderBills { get; set; } = new List<OrderBill>();
}
