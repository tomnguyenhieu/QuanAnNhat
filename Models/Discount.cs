using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("discounts")]
public partial class Discount
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("dish_id")]
    public int? DishId { get; set; }

    [Column("thumbnail")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Thumbnail { get; set; }

    [Column("discount_price")]
    public int? DiscountPrice { get; set; }

    [Column("time", TypeName = "datetime")]
    public DateTime? Time { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("DishId")]
    [InverseProperty("Discounts")]
    public virtual Dish? Dish { get; set; }

    [InverseProperty("Discount")]
    public virtual ICollection<OrderBill> OrderBills { get; set; } = new List<OrderBill>();
}
