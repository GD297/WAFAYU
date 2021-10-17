using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class wafayuContext : DbContext
    {
        public wafayuContext()
        {
        }

        public wafayuContext(DbContextOptions<wafayuContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Box> Boxes { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<PendingOrder> PendingOrders { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<SpacePackage> SpacePackages { get; set; }
        public virtual DbSet<Storage> Storages { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Box>(entity =>
            {
                entity.ToTable("Box");

                entity.Property(e => e.BoxCode).HasMaxLength(255);

                entity.Property(e => e.Position)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Size)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Shelf)
                    .WithMany(p => p.Boxes)
                    .HasForeignKey(d => d.ShelfId)
                    .HasConstraintName("FK_Box_Shelf");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => new { e.StorageId, e.OrderId });

                entity.ToTable("Feedback");

                entity.Property(e => e.Comment).HasMaxLength(255);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Order");

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.StorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Storage");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_User");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.StorageId)
                    .HasConstraintName("FK_Image_Storage");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.CustomerAvatar).HasMaxLength(255);

                entity.Property(e => e.CustomerName).HasMaxLength(255);

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaidTime).HasColumnType("datetime");

                entity.Property(e => e.PickupTime).HasColumnType("datetime");

                entity.Property(e => e.Size)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.OrderCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Order_User1");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.OrderOwners)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.BoxId, e.OrderId });

                entity.ToTable("OrderDetail");

                entity.Property(e => e.BoxCode)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Box)
                    .WithMany(p => p.OrderDetailBoxes)
                    .HasForeignKey(d => d.BoxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Box");

                entity.HasOne(d => d.BoxId2Navigation)
                    .WithMany(p => p.OrderDetailBoxId2Navigations)
                    .HasForeignKey(d => d.BoxId2)
                    .HasConstraintName("FK_OrderDetail_Box1");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<PendingOrder>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.SpacePackageId });

                entity.ToTable("PendingOrder");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.PendingOrders)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PendingOrder_Order");

                entity.HasOne(d => d.SpacePackage)
                    .WithMany(p => p.PendingOrders)
                    .HasForeignKey(d => d.SpacePackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PendingOrder_SpacePackage");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Shelf>(entity =>
            {
                entity.ToTable("Shelf");

                entity.Property(e => e.Size)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.Shelves)
                    .HasForeignKey(d => d.StorageId)
                    .HasConstraintName("FK_Shelf_Storage");
            });

            modelBuilder.Entity<SpacePackage>(entity =>
            {
                entity.ToTable("SpacePackage");

                entity.Property(e => e.BoxType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.SpacePackages)
                    .HasForeignKey(d => d.StorageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SpacePackage_Storage");
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.ToTable("Storage");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Picture).HasMaxLength(255);

                entity.Property(e => e.RejectedReason).HasMaxLength(255);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Storages)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_Storage_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Avatar)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Uid)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
