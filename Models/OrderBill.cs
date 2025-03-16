using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("order_bills")]
public partial class OrderBill
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("discount_id")]
    public int? DiscountId { get; set; }

    [Column("total_price")]
    public int? TotalPrice { get; set; }

    [Column("order_status")]
    public int? OrderStatus { get; set; }

    [Column("bill_status")]
    public int? BillStatus { get; set; }

    [Column("note")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Note { get; set; }

    [Column("time")]
    public byte[] Time { get; set; } = null!;

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("table_id")]
    public int? TableId { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("OrderBills")]
    public virtual Discount? Discount { get; set; }

    [InverseProperty("Orderbill")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("TableId")]
    [InverseProperty("OrderBills")]
    public virtual Table? Table { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("OrderBills")]
    public virtual User? User { get; set; }
}
