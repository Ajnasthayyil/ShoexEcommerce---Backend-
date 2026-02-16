//using Microsoft.EntityFrameworkCore;
//using ShoexEcommerce.Domain.Entities;
//using ShoexEcommerce.Domain.Enums;
//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

//namespace ShoexEcommerce.Infrastructure.Data
//{
//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        public DbSet<User> Users => Set<User>();
//        public DbSet<Role> Roles => Set<Role>();
//        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
//        public DbSet<Gender> Genders => Set<Gender>();
//        public DbSet<Brand> Brands => Set<Brand>();
//        public DbSet<Product> Products => Set<Product>();
//        public DbSet<Size> Sizes => Set<Size>();
//        public DbSet<ProductSize> ProductSizes => Set<ProductSize>();
//        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
//        public DbSet<Cart> Carts => Set<Cart>();
//        public DbSet<CartItem> CartItems => Set<CartItem>();
//        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
//        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
//        public DbSet<Order> Orders => Set<Order>();
//        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
//        public DbSet<Address> Addresses => Set<Address>();




//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            modelBuilder.Entity<User>()
//                        .HasIndex(x => x.Email)
//                        .IsUnique();

//            modelBuilder.Entity<Brand>()
//                        .HasIndex(x => x.Name)
//                        .IsUnique();

//            modelBuilder.Entity<Product>()
//                .Property(x => x.Price)
//                .HasPrecision(18, 2);

//            // RefreshToken relationship
//            modelBuilder.Entity<RefreshToken>()
//                .HasOne(x => x.User)
//                .WithMany(x => x.RefreshTokens)
//                .HasForeignKey(x => x.UserId)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<ProductSize>()
//            .HasKey(ps => new { ps.ProductId, ps.SizeId });

//            modelBuilder.Entity<ProductSize>()
//            .HasOne(ps => ps.Product)
//            .WithMany(p => p.ProductSizes)
//            .HasForeignKey(ps => ps.ProductId);

//            modelBuilder.Entity<ProductSize>()
//                .HasOne(ps => ps.Size)
//                .WithMany(s => s.ProductSizes)
//                .HasForeignKey(ps => ps.SizeId);

//            // ProductImage relationship
//            modelBuilder.Entity<ProductImage>()
//                .HasOne(pi => pi.Product)
//                .WithMany(p => p.Images)
//                .HasForeignKey(pi => pi.ProductId)
//                .OnDelete(DeleteBehavior.Cascade);

//            // ProductImage constraints
//            modelBuilder.Entity<ProductImage>()
//                .Property(x => x.Url)
//                .HasMaxLength(500)
//                .IsRequired();

//            modelBuilder.Entity<ProductImage>()
//                .Property(x => x.PublicId)
//                .HasMaxLength(200)
//                .IsRequired();

//            //cart 

//            modelBuilder.Entity<Cart>()
//                .HasIndex(c => c.UserId)
//                .IsUnique();


//            modelBuilder.Entity<CartItem>()
//                .HasIndex(i => new { i.CartId, i.ProductId, i.SizeId })
//                .IsUnique();

//            modelBuilder.Entity<CartItem>()
//                .Property(x => x.Quantity)
//                .HasDefaultValue(1);

//            //wishlist

//            modelBuilder.Entity<WishlistItem>()
//                .HasIndex(x => new { x.UserId, x.ProductId })
//                .IsUnique();

//            modelBuilder.Entity<PasswordResetOtp>(entity =>
//            {
//                entity.ToTable("PasswordResetOtps");
//            });

//            modelBuilder.Entity<Order>()
//            .Property(o => o.Status)
//            .HasConversion<int>();

//            modelBuilder.Entity<Order>(e =>
//            {
//                e.Property(x => x.SubTotal).HasPrecision(18, 2);
//                e.Property(x => x.TotalAmount).HasPrecision(18, 2);
//            });

//            modelBuilder.Entity<OrderItem>(e =>
//            {
//                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
//                e.Property(x => x.TotalPrice).HasPrecision(18, 2);
//            });

//            modelBuilder.Entity<ProductSize>(e =>
//            {
//                e.ToTable("ProductSizes");

//                e.HasKey(x => new { x.ProductId, x.SizeId });

//                e.Property(x => x.Stock)
//                    .HasDefaultValue(0);

//                e.HasOne(x => x.Product)
//                    .WithMany(p => p.ProductSizes)   
//                    .HasForeignKey(x => x.ProductId);

//                e.HasOne(x => x.Size)
//                    .WithMany(s => s.ProductSizes)   
//                    .HasForeignKey(x => x.SizeId);
//            });
//            modelBuilder.Entity<ProductSize>(e =>
//            {
//                e.ToTable("ProductSizes");

//                e.HasKey(x => new { x.ProductId, x.SizeId });

//                e.Property(x => x.Stock).HasDefaultValue(0);

//                e.HasOne(x => x.Product)
//                    .WithMany(p => p.ProductSizes)
//                    .HasForeignKey(x => x.ProductId)
//                    .OnDelete(DeleteBehavior.Cascade);

//                e.HasOne(x => x.Size)
//                    .WithMany(s => s.ProductSizes)
//                    .HasForeignKey(x => x.SizeId)
//                    .OnDelete(DeleteBehavior.Cascade);
//            });
//            modelBuilder.Entity<ProductSize>()
//                .HasKey(ps => new { ps.ProductId, ps.SizeId });

//            modelBuilder.Entity<ProductSize>()
//                .Property(ps => ps.Stock)
//                .HasDefaultValue(0);
//        }
//    }
//}
using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ShoexEcommerce.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Gender> Genders => Set<Gender>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<ProductSize> ProductSizes => Set<ProductSize>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

        public DbSet<PasswordResetOtp> PasswordResetOtps => Set<PasswordResetOtp>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Address> Addresses => Set<Address>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Brand>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            // RefreshToken relationship
            modelBuilder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //  ProductSize (ONLY ONCE) - stock per size
            modelBuilder.Entity<ProductSize>(e =>
            {
                e.ToTable("ProductSizes");

                e.HasKey(x => new { x.ProductId, x.SizeId });

                e.Property(x => x.Stock)
                    .HasDefaultValue(0);

                e.HasOne(x => x.Product)
                    .WithMany(p => p.ProductSizes)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Size)
                    .WithMany(s => s.ProductSizes)
                    .HasForeignKey(x => x.SizeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductImage relationship
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductImage constraints
            modelBuilder.Entity<ProductImage>()
                .Property(x => x.Url)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<ProductImage>()
                .Property(x => x.PublicId)
                .HasMaxLength(200)
                .IsRequired();

            // cart
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            modelBuilder.Entity<CartItem>()
                .HasIndex(i => new { i.CartId, i.ProductId, i.SizeId })
                .IsUnique();

            modelBuilder.Entity<CartItem>()
                .Property(x => x.Quantity)
                .HasDefaultValue(1);

            // wishlist
            modelBuilder.Entity<WishlistItem>()
                .HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique();

            modelBuilder.Entity<PasswordResetOtp>(entity =>
            {
                entity.ToTable("PasswordResetOtps");
            });

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Order>(e =>
            {
                e.Property(x => x.SubTotal).HasPrecision(18, 2);
                e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<OrderItem>(e =>
            {
                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
                e.Property(x => x.TotalPrice).HasPrecision(18, 2);
            });
        }
    }
}
