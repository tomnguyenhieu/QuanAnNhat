using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("dishlist")]
public partial class Dishlist
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("thumbnail")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Thumbnail { get; set; }

    [InverseProperty("Dishlist")]
    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
