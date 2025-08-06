using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ASP.net_8-Entity_Framework_RESTful_API.Models;

public partial class NorthwndContext : DbContext
{
    public NorthwndContext()
    {
    }

    public NorthwndContext(DbContextOptions<NorthwndContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TodoListItem> TodoListItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=SCRAPING\\SQLSERVERDEV;Database=NORTHWND;User Id=sa;Password=Abducted27641299;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<TodoListItem>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ItemId });

            entity.ToTable("TodoListItem");

            entity.Property(e => e.ItemName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
