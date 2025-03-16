using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("dish_id")]
    public int? DishId { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("total_price")]
    public int? TotalPrice { get; set; }

    [Column("orderbill_id")]
    public int? OrderbillId { get; set; }

    [ForeignKey("DishId")]
    [InverseProperty("Orders")]
    public virtual Dish? Dish { get; set; }

    [ForeignKey("OrderbillId")]
    [InverseProperty("Orders")]
    public virtual OrderBill? Orderbill { get; set; }
}
