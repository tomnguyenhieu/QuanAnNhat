using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("dishes")]
public partial class Dish
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("dishlist_id")]
    public int? DishlistId { get; set; }

    [Column("name")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("thumbnail")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Thumbnail { get; set; }

    [Column("price")]
    public int? Price { get; set; }

    [Column("description")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Description { get; set; }

    [Column("size")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Size { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("must_try")]
    public int? MustTry { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("total_sold")]
    public int? TotalSold { get; set; }

    [InverseProperty("Dish")]
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    [ForeignKey("DishlistId")]
    [InverseProperty("Dishes")]
    public virtual Dishlist? Dishlist { get; set; }

    [InverseProperty("Dish")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Dish")]
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
