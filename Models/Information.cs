using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("informations")]
public partial class Information
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("birth")]
    public DateOnly? Birth { get; set; }

    [Column("gender")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Gender { get; set; }

    [Column("phone")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column("address")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Address { get; set; }

    [Column("citizen_id")]
    public int? CitizenId { get; set; }

    [InverseProperty("Information")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("Information")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
