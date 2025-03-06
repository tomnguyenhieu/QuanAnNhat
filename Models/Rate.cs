using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("rates")]
public partial class Rate
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("comment")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Comment { get; set; }

    [Column("image")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Image { get; set; }

    [Column("time")]
    public DateOnly? Time { get; set; }

    [Column("type")]
    public int? Type { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Rates")]
    public virtual User? User { get; set; }
}
