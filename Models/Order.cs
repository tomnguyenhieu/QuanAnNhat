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

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("dish_id")]
    public int? DishId { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("table_id")]
    public int? TableId { get; set; }

    [Column("time", TypeName = "datetime")]
    public DateTime? Time { get; set; }

    [Column("note")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Note { get; set; }

    [Column("total_price")]
    public int? TotalPrice { get; set; }

    [ForeignKey("DishId")]
    [InverseProperty("Orders")]
    public virtual Dish? Dish { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderBill> OrderBills { get; set; } = new List<OrderBill>();

    [ForeignKey("TableId")]
    [InverseProperty("Orders")]
    public virtual Table? Table { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
