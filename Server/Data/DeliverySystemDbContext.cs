using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public partial class DeliverySystemDbContext : DbContext
{
    public DeliverySystemDbContext()
    {
    }

    public DeliverySystemDbContext(DbContextOptions<DeliverySystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Courier> Couriers { get; set; }

    public virtual DbSet<CourierDocument> CourierDocuments { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

    public virtual DbSet<DeliveryStatus> DeliveryStatuses { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<OperationLog> OperationLogs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderedItem> OrderedItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDocument> ProductDocuments { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-I3ITG5J;Database=DeliverySystem;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F3BE377CC33A");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__F2D21A96D35948AF");

            entity.HasOne(d => d.Region).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__RegionID__6D0D32F4");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A0408A65C67");

            entity.HasOne(d => d.City).WithMany(p => p.Clients).HasConstraintName("FK__Clients__CityID__6EF57B66");

            entity.HasOne(d => d.Region).WithMany(p => p.Clients).HasConstraintName("FK__Clients__RegionI__6E01572D");
        });

        modelBuilder.Entity<Courier>(entity =>
        {
            entity.HasKey(e => e.CourierId).HasName("PK__Couriers__CDAE76F6C2BA3802");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Couriers).HasConstraintName("FK__Couriers__Vehicl__49C3F6B7");
        });

        modelBuilder.Entity<CourierDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__CourierD__1ABEEF6F948201D2");

            entity.HasOne(d => d.Courier).WithMany(p => p.CourierDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourierDo__Couri__4D94879B");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__Deliveri__626D8FEE62D9F4CD");

            entity.HasOne(d => d.Courier).WithMany(p => p.Deliveries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Deliverie__Couri__5629CD9C");

            entity.HasOne(d => d.Order).WithMany(p => p.Deliveries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Deliverie__Order__5535A963");

            entity.HasOne(d => d.Status).WithMany(p => p.Deliveries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Deliverie__Statu__571DF1D5");
        });

        modelBuilder.Entity<DeliveryAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Delivery__091C2A1B4583A0B5");

            entity.HasOne(d => d.Order).WithMany(p => p.DeliveryAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DeliveryA__Order__06CD04F7");

            entity.HasOne(d => d.User).WithMany(p => p.DeliveryAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DeliveryA__UserI__05D8E0BE");
        });

        modelBuilder.Entity<DeliveryStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Delivery__C8EE2043BCACC2D3");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD5AB310244");

            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.Invoices).HasConstraintName("FK__Invoices__Produc__17036CC0");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PK__Models__E8D7A1CC7E69BBBD");

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Models__BrandID__398D8EEE");
        });

        modelBuilder.Entity<OperationLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Operatio__5E5499A86FE84046");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.OperationLogs).HasConstraintName("FK__Operation__UserI__0A9D95DB");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFBB73D3B9");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders).HasConstraintName("FK__Orders__ClientID__6FE99F9F");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderCreatedByNavigations).HasConstraintName("FK__Orders__CreatedB__71D1E811");

            entity.HasOne(d => d.ManagedByNavigation).WithMany(p => p.OrderManagedByNavigations).HasConstraintName("FK__Orders__ManagedB__72C60C4A");
        });

        modelBuilder.Entity<OrderedItem>(entity =>
        {
            entity.HasKey(e => e.OrderedItemId).HasName("PK__OrderedI__E888B7CB5C808A1C");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderedIt__Order__5070F446");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderedItems).HasConstraintName("FK__OrderedIt__Produ__02FC7413");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDC563E08E");

            entity.Property(e => e.CreateAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products).HasConstraintName("FK__Products__Produc__14270015");
        });

        modelBuilder.Entity<ProductDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__ProductD__1ABEEF6FC98CD3FF");

            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDocuments).HasConstraintName("FK__ProductDo__Produ__1AD3FDA4");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F4E17849E49");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__Regions__ACD84443204662C0");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AEF4AD312E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Courier).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__Courier__0E6E26BF");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__OrderID__0F624AF8");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserID__0D7A0286");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3ADE039D6C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC5FCE50C8");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK__Users__RoleID__6477ECF3");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B54B2ECD16FF0");

            entity.HasOne(d => d.Model).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicles__ModelI__3E52440B");

            entity.HasOne(d => d.Type).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicles__TypeID__3F466844");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__VehicleT__516F03954A090D4D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
