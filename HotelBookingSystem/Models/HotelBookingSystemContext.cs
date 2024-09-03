using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;

public partial class HotelBookingSystemContext : DbContext
{
    public HotelBookingSystemContext()
    {
    }

    public HotelBookingSystemContext(DbContextOptions<HotelBookingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<HotelRevenueReport> HotelRevenueReports { get; set; }



    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=YOUSEF\\SQLEXPRESS;Initial Catalog=HotelBookingSystemDB;Integrated Security=True;Trust Server Certificate=True;Command Timeout=300");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hotels__3214EC07714D0882");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reports__3214EC076C7AD694");

            entity.HasOne(d => d.Admin).WithMany(p => p.Reports).HasConstraintName("FK__Reports__AdminId__5AEE82B9");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3214EC070E47E1EF");

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations).HasConstraintName("FK__Reservati__RoomI__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations).HasConstraintName("FK__Reservati__UserI__52593CB8");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC07373EDA37");

            entity.Property(e => e.AvailabilityStatus).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms).HasConstraintName("FK__Rooms__HotelId__4F7CD00D");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Testimon__3214EC07DE049C76");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Testimonials).HasConstraintName("FK__Testimoni__Hotel__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials).HasConstraintName("FK__Testimoni__UserI__571DF1D5");
        });





        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FC984A3E");
        });

        OnModelCreatingPartial(modelBuilder);





        // تحديد HotelRevenueReport ككيان ليس له مفتاح أساسي
        modelBuilder.Entity<HotelRevenueReport>()
            .HasNoKey();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
