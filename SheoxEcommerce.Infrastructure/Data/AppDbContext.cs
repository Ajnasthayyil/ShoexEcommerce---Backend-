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
    // Main EF Core DbContext class
    public class AppDbContext : DbContext
    {
        // DbContext constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        
        // DbSet declarations, Each DbSet represents a table in the database
        

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

       
        // Fluent API configurations
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            // Email must be unique
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            
            // Brand name must be unique
            modelBuilder.Entity<Brand>()
                .HasIndex(x => x.Name)
                .IsUnique();

           
            // Set decimal precision for product price
            modelBuilder.Entity<Product>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            // One user can have many refresh tokens, Cascade delete when user is removed
            modelBuilder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prodict size (Stock per Size) 
            modelBuilder.Entity<ProductSize>(entity =>
            {
                entity.ToTable("ProductSizes");

                entity.HasKey(x => new { x.ProductId, x.SizeId });

                // Default stock value
                entity.Property(x => x.Stock)
                    .HasDefaultValue(0);

                // Product - ProductSizes relationship
                entity.HasOne(x => x.Product)
                    .WithMany(p => p.ProductSizes)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Size -  ProductSizes relationship
                entity.HasOne(x => x.Size)
                    .WithMany(s => s.ProductSizes)
                    .HasForeignKey(x => x.SizeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // One product can have many images
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Image URL validation
            modelBuilder.Entity<ProductImage>()
                .Property(x => x.Url)
                .HasMaxLength(500)
                .IsRequired();

            // Cloudinary PublicId validation
            modelBuilder.Entity<ProductImage>()
                .Property(x => x.PublicId)
                .HasMaxLength(200)
                .IsRequired();

            // One cart per user
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            // Prevent duplicate cart items
            modelBuilder.Entity<CartItem>()
                .HasIndex(i => new { i.CartId, i.ProductId, i.SizeId })
                .IsUnique();

            // Default quantity is 1
            modelBuilder.Entity<CartItem>()
                .Property(x => x.Quantity)
                .HasDefaultValue(1);

            // Prevent duplicate wishlist items per user
            modelBuilder.Entity<WishlistItem>()
                .HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique();

            // Password reset OTP
            modelBuilder.Entity<PasswordResetOtp>(entity =>
            {
                entity.ToTable("PasswordResetOtps");
            });

          
            // Store enum status as integer
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<int>();

            // Order price precision
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(x => x.SubTotal).HasPrecision(18, 2);
                entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
            });

            // Order item price precision
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
                entity.Property(x => x.TotalPrice).HasPrecision(18, 2);
            });
        }
    }
}