using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanAnNhat.Models;

namespace QuanAnNhat.DBContext;

public partial class QuanannhatContext : DbContext
{
    public QuanannhatContext()
    {
    }

    public QuanannhatContext(DbContextOptions<QuanannhatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Dish> Dishes { get; set; }

    public virtual DbSet<Dishlist> Dishlists { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Information> Informations { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<IngredientBill> IngredientBills { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderBill> OrderBills { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<SalaryBill> SalaryBills { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=quanannhat;Persist Security Info=True;User ID=sa;Password=sa;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__discount__3213E83F2755CFF3");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Dish).WithMany(p => p.Discounts).HasConstraintName("FK__discounts__dish___5165187F");
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dishes__3213E83FC2197876");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Dishlist).WithMany(p => p.Dishes).HasConstraintName("FK__dishes__dishlist__52593CB8");
        });

        modelBuilder.Entity<Dishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dishlist__3213E83F2CC23AD3");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__employee__3213E83F960C225F");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Information).WithMany(p => p.Employees).HasConstraintName("FK__employees__infor__534D60F1");
        });

        modelBuilder.Entity<Information>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__informat__3213E83F5E74BB76");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ingredie__3213E83F53488948");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<IngredientBill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ingredie__3213E83F3B837A11");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Ingredient).WithMany(p => p.IngredientBills).HasConstraintName("FK__ingredien__ingre__5441852A");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3213E83F013DBEB1");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Dish).WithMany(p => p.Orders).HasConstraintName("FK__orders__dish_id__5812160E");

            entity.HasOne(d => d.Orderbill).WithMany(p => p.Orders).HasConstraintName("FK__orders__orderbil__59063A47");
        });

        modelBuilder.Entity<OrderBill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_bi__3213E83F645C3512");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Time).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Discount).WithMany(p => p.OrderBills).HasConstraintName("FK__order_bil__disco__6FE99F9F");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderBills).HasConstraintName("FK__order_bil__table__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.OrderBills).HasConstraintName("FK__order_bil__user___71D1E811");
        });

        modelBuilder.Entity<Rate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__rates__3213E83F0C420FA2");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.Rates).HasConstraintName("FK__rates__user_id__59FA5E80");
        });

        modelBuilder.Entity<SalaryBill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__salary_b__3213E83F30EE6F18");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Employee).WithMany(p => p.SalaryBills).HasConstraintName("FK__salary_bi__emplo__5AEE82B9");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tables__3213E83FD5C886AE");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FF520A72E");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Information).WithMany(p => p.Users).HasConstraintName("FK__users__informati__5BE2A6F2");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__wishlist__3213E83F4647A4EA");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Dish).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__dish_i__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__user_i__5DCAEF64");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
