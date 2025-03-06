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

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("discount_id")]
    public int? DiscountId { get; set; }

    [Column("total_price")]
    public int? TotalPrice { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("OrderBills")]
    public virtual Discount? Discount { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderBills")]
    public virtual Order? Order { get; set; }
}
