using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanAnNhat.Models;

[Table("salary_bills")]
public partial class SalaryBill
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employee_id")]
    public int? EmployeeId { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("total_shifts")]
    public int? TotalShifts { get; set; }

    [Column("time")]
    public DateOnly? Time { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("SalaryBills")]
    public virtual Employee? Employee { get; set; }
}
