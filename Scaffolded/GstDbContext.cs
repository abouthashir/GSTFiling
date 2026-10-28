using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Scaffolded;

public partial class GstDbContext : DbContext
{
    public GstDbContext(DbContextOptions<GstDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<tbl_Status> tbl_Status { get; set; }

    public virtual DbSet<tbl_aboutus> tbl_aboutus { get; set; }

    public virtual DbSet<tbl_adminroles> tbl_adminroles { get; set; }

    public virtual DbSet<tbl_adminuser> tbl_adminuser { get; set; }

    public virtual DbSet<tbl_centre> tbl_centre { get; set; }

    public virtual DbSet<tbl_consultant> tbl_consultant { get; set; }

    public virtual DbSet<tbl_ctype> tbl_ctype { get; set; }

    public virtual DbSet<tbl_customer> tbl_customer { get; set; }

    public virtual DbSet<tbl_district> tbl_district { get; set; }

    public virtual DbSet<tbl_headercontent> tbl_headercontent { get; set; }

    public virtual DbSet<tbl_homebanner> tbl_homebanner { get; set; }

    public virtual DbSet<tbl_homecontent> tbl_homecontent { get; set; }

    public virtual DbSet<tbl_invconsultant> tbl_invconsultant { get; set; }

    public virtual DbSet<tbl_invmonth> tbl_invmonth { get; set; }

    public virtual DbSet<tbl_invoicebill> tbl_invoicebill { get; set; }

    public virtual DbSet<tbl_invoicepdf> tbl_invoicepdf { get; set; }

    public virtual DbSet<tbl_invoices> tbl_invoices { get; set; }

    public virtual DbSet<tbl_month> tbl_month { get; set; }

    public virtual DbSet<tbl_msg> tbl_msg { get; set; }

    public virtual DbSet<tbl_pages> tbl_pages { get; set; }

    public virtual DbSet<tbl_roleaccess> tbl_roleaccess { get; set; }

    public virtual DbSet<tbl_roles> tbl_roles { get; set; }

    public virtual DbSet<tbl_socialmedia> tbl_socialmedia { get; set; }

    public virtual DbSet<tbl_taxrate> tbl_taxrate { get; set; }

    public virtual DbSet<tbl_test> tbl_test { get; set; }

    public virtual DbSet<tbl_update> tbl_update { get; set; }

    public virtual DbSet<tbl_year> tbl_year { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tbl_Status>(entity =>
        {
            entity.Property(e => e.stid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_aboutus>(entity =>
        {
            entity.Property(e => e.abtid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_adminroles>(entity =>
        {
            entity.Property(e => e.roleid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_adminuser>(entity =>
        {
            entity.Property(e => e.userid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_centre>(entity =>
        {
            entity.Property(e => e.cid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_consultant>(entity =>
        {
            entity.Property(e => e.cnid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_ctype>(entity =>
        {
            entity.Property(e => e.ctypid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_customer>(entity =>
        {
            entity.Property(e => e.cstmrid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_district>(entity =>
        {
            entity.Property(e => e.did).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_headercontent>(entity =>
        {
            entity.Property(e => e.hid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_homebanner>(entity =>
        {
            entity.Property(e => e.bnrid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_homecontent>(entity =>
        {
            entity.Property(e => e.midid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_invconsultant>(entity =>
        {
            entity.Property(e => e.ascnsltid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_invmonth>(entity =>
        {
            entity.Property(e => e.cmid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_invoicebill>(entity =>
        {
            entity.Property(e => e.bid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_invoicepdf>(entity =>
        {
            entity.Property(e => e.invexid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_invoices>(entity =>
        {
            entity.Property(e => e.invid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_month>(entity =>
        {
            entity.Property(e => e.mid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_msg>(entity =>
        {
            entity.Property(e => e.msgid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_pages>(entity =>
        {
            entity.Property(e => e.pgid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_roleaccess>(entity =>
        {
            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_roles>(entity =>
        {
            entity.Property(e => e.roleid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_socialmedia>(entity =>
        {
            entity.Property(e => e.socialid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_taxrate>(entity =>
        {
            entity.Property(e => e.txid).ValueGeneratedNever();
        });

        modelBuilder.Entity<tbl_year>(entity =>
        {
            entity.Property(e => e.yid).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
