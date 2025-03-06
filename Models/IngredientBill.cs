using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("ingredient_bills")]
public partial class IngredientBill
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ingredient_id")]
    public int? IngredientId { get; set; }

    [Column("price")]
    public int? Price { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("total_price")]
    public int? TotalPrice { get; set; }

    [Column("time", TypeName = "datetime")]
    public DateTime? Time { get; set; }

    [ForeignKey("IngredientId")]
    [InverseProperty("IngredientBills")]
    public virtual Ingredient? Ingredient { get; set; }
}
