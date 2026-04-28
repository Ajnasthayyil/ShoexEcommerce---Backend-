# 🛍️ Shoex Ecommerce Backend API

> A **production-grade REST API** for an ecommerce platform built with **ASP.NET Core** following **Clean Architecture** principles. Demonstrates professional backend development with JWT authentication, role-based authorization, EF Core, migrations, Cloudinary integration, and comprehensive API documentation.

<p align="center">
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
  <img src="https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core"/>
  <img src="https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white" alt="JWT"/>
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=white" alt="Swagger"/>
</p>

---

## 📋 Table of Contents
- [Overview](#overview)
- [Architecture](#architecture)
- [Backend Flow (Interview Explanation)](#backend-flow-interview-explanation)
- [Tech Stack](#tech-stack)
- [Key Concepts](#key-concepts)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Layers Explained](#api-layers-explained)
- [Authentication & Authorization](#authentication--authorization)
- [Database & EF Core](#database--ef-core)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Deployment](#deployment)
- [Troubleshooting](#troubleshooting)
- [Contact](#contact)

---

## 🎯 Overview

Shoex Backend is a **complete backend solution** for the ecommerce platform, demonstrating:

✅ **Clean Architecture** — Strict layer separation for maintainability  
✅ **SOLID Principles** — Professional code organization  
✅ **JWT Authentication** — Secure user authentication with role-based access  
✅ **EF Core Migrations** — Database versioning & seeding  
✅ **Cloudinary Integration** — Cloud image storage for products  
✅ **Comprehensive DTOs** — Safe data transfer between layers  
✅ **Role-Based Authorization** — Admin, Customer, Staff roles  
✅ **Swagger Documentation** — Interactive API docs  

---

## 🏗️ Architecture

### Clean Architecture: 4-Layer Design

```
┌─────────────────────────────────────────────────────────────┐
│  API Layer (Presentation)                                   │
│  ↓ Controllers receive HTTP requests                        │
│  ↓ Validates input, calls Application Layer                 │
│  └─ Returns JSON responses                                  │
├─────────────────────────────────────────────────────────────┤
│  Application Layer (Business Logic Contracts)               │
│  ↓ Defines interfaces (IProductService, IOrderService)      │
│  ↓ Contains DTOs for data transfer                          │
│  ↓ Orchestrates business logic                              │
│  └─ No direct database access                               │
├─────────────────────────────────────────────────────────────┤
│  Infrastructure Layer (Implementation)                      │
│  ↓ Implements interfaces from Application Layer             │
│  ↓ Directly accesses database via EF Core                   │
│  ↓ Handles Cloudinary, Email, external services            │
│  └─ Database context & repositories                         │
├─────────────────────────────────────────────────────────────┤
│  Domain Layer (Core Business Rules)                         │
│  ↓ Pure C# classes (no framework dependencies)              │
│  ↓ Core entities (User, Product, Order)                     │
│  ↓ Business logic & validation                              │
│  └─ Independent from other layers                           │
└─────────────────────────────────────────────────────────────┘
```

### Benefits of This Architecture

| Benefit | Why It Matters |
|---------|---|
| **Testability** | Each layer can be tested in isolation |
| **Maintainability** | Changes isolated to specific layers |
| **Scalability** | Easy to add new features without breaking existing code |
| **Flexibility** | Can swap implementations (SQL Server → PostgreSQL) |
| **Reusability** | Business logic can be reused across different APIs |
| **Dependency Inversion** | High-level modules don't depend on low-level modules |

---

## 📊 Backend Flow (Interview Explanation)

### Complete Request-Response Flow ⭐

```
1️⃣  HTTP Request Arrives at Controller
    ├─ POST /api/products
    ├─ Headers: Authorization: Bearer {JWT_TOKEN}
    └─ Body: { "name": "Nike Air", "price": 120 }

2️⃣  API Layer (ShoexEcommerce.API)
    ├─ ProductController receives request
    ├─ Validates JWT token via AuthMiddleware
    ├─ Validates request data (ModelState)
    └─ Calls → Application Layer Service

3️⃣  Application Layer (ShoexEcommerce.Application)
    ├─ IProductService interface defines contract
    ├─ Implements business logic:
    │  ├─ Check user role (Admin only)
    │  ├─ Validate product data
    │  ├─ Check if product already exists
    │  └─ Map DTO → Domain Entity
    └─ Calls → Infrastructure Layer

4️⃣  Infrastructure Layer (ShoexEcommerce.Infrastructure)
    ├─ ProductService implements IProductService
    ├─ Uses ShoexDbContext (EF Core)
    ├─ Cloudinary service for image storage
    ├─ Saves to database via DbSet
    └─ Returns saved entity

5️⃣  Domain Layer (ShoexEcommerce.Domain)
    ├─ Product entity is pure C# class
    ├─ Contains business rules:
    │  ├─ Price validation
    │  ├─ Stock validation
    │  └─ Product creation logic
    └─ No dependencies on other layers

6️⃣  Database Transaction
    ├─ EF Core generates SQL
    ├─ Executes INSERT into Products table
    ├─ Commits transaction
    └─ Returns database-generated ID

7️⃣  Response Back Through Layers
    ├─ Infrastructure returns entity
    ├─ Application maps Entity → DTO
    ├─ API Controller wraps DTO in response
    └─ JSON sent to client

8️⃣  Client Receives Response
    ├─ Status: 201 Created
    ├─ Body: { "id": 1, "name": "Nike Air", "price": 120 }
    └─ Frontend processes and updates UI
```

### Why This Flow Matters for Interviews

✅ **Shows separation of concerns** — Each layer has single responsibility  
✅ **Demonstrates SOLID principles** — Especially Dependency Inversion  
✅ **Scalable** — Can add logging, caching, validation at each layer  
✅ **Testable** — Mock dependencies easily  
✅ **Professional** — Enterprise-grade architecture  

---

## 🛠 Tech Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **API** | ASP.NET Core 6/7/8 | Web framework |
| **Language** | C# 11 | Type-safe, modern language |
| **Database** | SQL Server | Relational database |
| **ORM** | Entity Framework Core | Database access & migrations |
| **Authentication** | JWT (JSON Web Tokens) | Stateless authentication |
| **Image Storage** | Cloudinary | Cloud image management |
| **Documentation** | Swagger/OpenAPI | Interactive API docs |
| **Email/SMS** | External providers | Notifications |
| **Dependency Injection** | Built-in ASP.NET Core | IoC container |

---

## 🔑 Key Concepts

### 1. **Clean Architecture**
- 4 independent, loosely-coupled layers
- Domain layer has NO external dependencies
- Dependency flows inward (API → Domain)

### 2. **DTO Pattern (Data Transfer Objects)**
```csharp
// DTOs are used between layers
// User doesn't see database entities

// Domain Layer
public class Product  // Entity (database representation)
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Application Layer
public class CreateProductDto  // DTO (API input)
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductDto  // DTO (API output)
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### 3. **JWT Authentication**
```csharp
// Login generates JWT token
[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    // Verify credentials
    var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
    
    if (user == null)
        return Unauthorized("Invalid credentials");
    
    // Generate JWT
    var token = _jwtService.GenerateToken(user);
    
    return Ok(new { accessToken = token, userId = user.Id });
}

// Token sent with every request
// Headers: Authorization: Bearer {token}
```

### 4. **Role-Based Authorization**
```csharp
// Controllers/endpoints protected by role
[Authorize(Roles = "Admin")]
[HttpPost]
public async Task<IActionResult> CreateProduct(CreateProductDto dto)
{
    // Only Admin can create products
    var product = await _productService.CreateAsync(dto);
    return Created($"/api/products/{product.Id}", product);
}
```

### 5. **EF Core Migrations**
```bash
# Create migration when you add/modify entities
dotnet ef migrations add AddProductEntity

# Apply migrations to database
dotnet ef database update

# EF Core generates SQL automatically
```

### 6. **Cloudinary Integration**
```csharp
// Images uploaded to Cloudinary (cloud storage)
public async Task<string> UploadImageAsync(IFormFile file)
{
    var uploadParams = new ImageUploadParams()
    {
        File = new FileDescription(file.FileName, file.OpenReadStream())
    };
    
    var result = await _cloudinary.UploadAsync(uploadParams);
    return result.SecureUrl.ToString();  // Cloud URL
}
```

---

## 📁 Project Structure

### **ShoexEcommerce.Domain** 🎯 (Business Rules)

```
Domain/
├── Entities/
│   ├── User.cs              # User entity with roles
│   ├── Product.cs           # Product with inventory
│   ├── Order.cs             # Order management
│   ├── OrderItem.cs         # Order line items
│   ├── Cart.cs              # Shopping cart
│   ├── CartItem.cs          # Cart items
│   ├── Review.cs            # Product reviews
│   └── Payment.cs           # Payment records
│
├── Enums/
│   ├── OrderStatus.cs       # Pending, Processing, Shipped...
│   ├── PaymentStatus.cs     # Completed, Failed, Pending...
│   ├── UserRole.cs          # Admin, Customer, Staff
│   └── PaymentMethod.cs     # Card, PayPal, etc.
│
└── Common/
    ├── BaseEntity.cs        # Id, CreatedDate, UpdatedDate
    └── Result.cs            # Success/failure wrapper
```

**Key Point:** These are PURE C# classes with NO Entity Framework attributes. They define business rules.

---

### **ShoexEcommerce.Application** 📋 (Contracts & DTOs)

```
Application/
├── Interfaces/
│   ├── IProductService.cs
│   │   ├── GetAllAsync()
│   │   ├── GetByIdAsync(id)
│   │   ├── CreateAsync(dto)
│   │   ├── UpdateAsync(id, dto)
│   │   └── DeleteAsync(id)
│   │
│   ├── IOrderService.cs
│   ├── IAuthService.cs
│   ├── IUserService.cs
│   ├── ICartService.cs
│   └── IPaymentService.cs
│
├── DTOs/
│   ├── ProductDto.cs
│   ├── CreateProductDto.cs
│   ├── UpdateProductDto.cs
│   ├── OrderDto.cs
│   ├── CreateOrderDto.cs
│   ├── UserDto.cs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── CartDto.cs
│
└── Common/
    ├── Mappings/            # Entity ↔ DTO mappings
    ├── Validators/          # Fluent validation rules
    └── Constants.cs         # Error messages, status codes
```

**Key Point:** Defines WHAT to do (interfaces), not HOW to do it.

---

### **ShoexEcommerce.Infrastructure** ⚙️ (Implementation & Data Access)

```
Infrastructure/
├── Data/
│   ├── ShoexDbContext.cs    # ⭐ Main DbContext
│   │   ├── DbSet<Product>
│   │   ├── DbSet<Order>
│   │   ├── DbSet<User>
│   │   └── OnModelCreating() // Seeding data
│   │
│   ├── Migrations/
│   │   ├── 20240101000000_InitialCreate.cs
│   │   ├── 20240105000000_AddProductTable.cs
│   │   └── 20240110000000_AddOrderTable.cs
│   │
│   └── SeedData.cs          # Initial data seeding
│
├── Services/
│   ├── ProductService.cs    # Implements IProductService
│   │   ├── Uses DbContext
│   │   ├── Calls Cloudinary for images
│   │   └── Returns DTOs
│   │
│   ├── OrderService.cs      # Implements IOrderService
│   ├── UserService.cs       # Implements IUserService
│   ├── CartService.cs       # Implements ICartService
│   └── PaymentService.cs    # Implements IPaymentService
│
├── Security/
│   ├── JwtService.cs        # Generate/validate JWT tokens
│   └── PasswordHasher.cs    # Secure password hashing
│
└── External/
    ├── CloudinaryService.cs # Image upload to Cloudinary
    ├── EmailService.cs      # Send emails
    └── SmsService.cs        # Send SMS notifications
```

**Key Point:** Implements interfaces defined in Application layer.

---

### **ShoexEcommerce.API** 🌐 (Presentation)

```
API/
├── Controllers/
│   ├── ProductsController.cs
│   │   ├── GET /api/products              # Get all
│   │   ├── GET /api/products/{id}         # Get by ID
│   │   ├── POST /api/products             # Create (Admin)
│   │   ├── PUT /api/products/{id}         # Update (Admin)
│   │   └── DELETE /api/products/{id}      # Delete (Admin)
│   │
│   ├── OrdersController.cs
│   │   ├── GET /api/orders                # User's orders
│   │   ├── POST /api/orders               # Create order
│   │   └── GET /api/orders/{id}           # Order details
│   │
│   ├── AuthController.cs
│   │   ├── POST /api/auth/register        # Register
│   │   ├── POST /api/auth/login           # Login
│   │   └── POST /api/auth/refresh-token   # Refresh JWT
│   │
│   ├── CartController.cs
│   ├── PaymentController.cs
│   └── AdminController.cs
│
├── Middleware/
│   ├── AuthMiddleware.cs    # Validate JWT token
│   ├── ErrorHandlingMiddleware.cs  # Global error handling
│   └── LoggingMiddleware.cs # Request/response logging
│
├── Extensions/
│   ├── ServiceCollectionExtensions.cs  # Register services
│   └── ApplicationBuilderExtensions.cs # Configure middleware
│
├── Program.cs               # ⭐ Configuration & startup
│   ├── Add DbContext
│   ├── Add JWT options
│   ├── Register services (Dependency Injection)
│   ├── Configure middleware
│   └── Build & run
│
└── appsettings.json         # Configuration
    ├── ConnectionStrings
    ├── JWT settings
    ├── Cloudinary settings
    └── Email/SMS settings
```

**Key Point:** Controllers are THIN. They call services and return responses.

---

## 🚀 Getting Started

### Prerequisites

```bash
# Check versions
dotnet --version        # .NET 6 SDK or higher
sqlserver               # SQL Server installed
```

### Installation

```bash
# 1. Clone repository
git clone https://github.com/Ajnasthayyil/ShoexEcommerce---Backend-.git
cd ShoexEcommerce---Backend-

# 2. Restore NuGet packages
dotnet restore

# 3. Configure database connection
# Edit: appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ShoexDb;User Id=sa;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars-long",
    "ExpirationMinutes": 60
  }
}

# 4. Apply migrations to create database
dotnet ef database update

# 5. Start API server
dotnet run

# API runs at: https://localhost:5001
# Swagger docs: https://localhost:5001/swagger/index.html
```

---

## 🔌 API Layers Explained

### Layer 1: API Controller (Receives Requests)

```csharp
// ShoexEcommerce.API/Controllers/ProductsController.cs

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService)
    {
        _productService = productService;  // Injected
    }
    
    // ✅ Client calls this endpoint
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // ✅ Admin creates product
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return Created($"/api/products/{product.Id}", product);
    }
}
```

**Responsibility:** Receive HTTP request, validate, call service, return response.

---

### Layer 2: Application Service (Business Logic)

```csharp
// ShoexEcommerce.Application/Interfaces/IProductService.cs

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}

// ShoexEcommerce.Infrastructure/Services/ProductService.cs

public class ProductService : IProductService
{
    private readonly ShoexDbContext _context;
    private readonly ICloudinaryService _cloudinary;
    
    public ProductService(
        ShoexDbContext context,
        ICloudinaryService cloudinary)
    {
        _context = context;
        _cloudinary = cloudinary;
    }
    
    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        // ✅ Business Logic Here
        
        // 1. Validate input
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Product name required");
        
        if (dto.Price <= 0)
            throw new ArgumentException("Price must be > 0");
        
        // 2. Check if product already exists
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.Name == dto.Name);
        
        if (existingProduct != null)
            throw new InvalidOperationException("Product already exists");
        
        // 3. Upload image to Cloudinary if provided
        string imageUrl = null;
        if (dto.Image != null)
        {
            imageUrl = await _cloudinary.UploadImageAsync(dto.Image);
        }
        
        // 4. Create domain entity
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = imageUrl,
            CategoryId = dto.CategoryId,
            CreatedDate = DateTime.UtcNow
        };
        
        // 5. Save to database
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        // 6. Return DTO (never return entity directly)
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl
        };
    }
}
```

**Responsibility:** Implement business logic, validate, interact with database.

---

### Layer 3: Domain Entity (Business Rules)

```csharp
// ShoexEcommerce.Domain/Entities/Product.cs

public class Product : BaseEntity  // Inherits Id, CreatedDate
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; }
    public int CategoryId { get; set; }
    
    // Navigation properties
    public Category Category { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    
    // ✅ Business Logic in Domain
    public bool IsInStock() => Stock > 0;
    
    public void ReduceStock(int quantity)
    {
        if (Stock < quantity)
            throw new InvalidOperationException("Insufficient stock");
        
        Stock -= quantity;
    }
    
    public decimal GetDiscountedPrice(decimal discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException("Invalid discount");
        
        return Price * (1 - (discountPercent / 100));
    }
}

// Base entity class (reused across all entities)
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
}
```

**Responsibility:** Define entity structure, contain business rules, NO external dependencies.

---

## 🔐 Authentication & Authorization

### JWT Token Flow

```csharp
// 1. User logs in
[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
    if (user == null) return Unauthorized();
    
    // 2. Generate JWT token
    var token = _jwtService.GenerateToken(user);
    
    return Ok(new
    {
        accessToken = token,
        userId = user.Id,
        role = user.Role
    });
}

// 3. Token stored on client (localStorage)
// 4. Sent with every request
// GET /api/products
// Headers: Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

// 5. Middleware validates token
public class AuthMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            var userId = _jwtService.ValidateToken(token);
            if (userId != null)
            {
                context.Items["UserId"] = userId;
            }
        }
        
        await _next(context);
    }
}

// 6. Protect endpoints with [Authorize]
[Authorize]
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(int id)
{
    var product = await _productService.GetByIdAsync(id);
    return Ok(product);
}

// 7. Admin-only endpoints
[Authorize(Roles = "Admin")]
[HttpPost]
public async Task<IActionResult> CreateProduct(CreateProductDto dto)
{
    var product = await _productService.CreateAsync(dto);
    return Created($"/api/products/{product.Id}", product);
}
```

---

## 💾 Database & EF Core

### DbContext Configuration

```csharp
// ShoexEcommerce.Infrastructure/Data/ShoexDbContext.cs

public class ShoexDbContext : DbContext
{
    public ShoexDbContext(DbContextOptions<ShoexDbContext> options)
        : base(options) { }
    
    // ✅ DbSets define database tables
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // ✅ Relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // ✅ Constraints & defaults
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);
        
        // ✅ Seed initial data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Add default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Running Shoes" },
            new Category { Id = 2, Name = "Casual Shoes" }
        );
    }
}
```

### Migrations

```bash
# Create a migration when you change entities
dotnet ef migrations add AddProductEntity
# Creates: Migrations/20240101000000_AddProductEntity.cs

# Apply to database
dotnet ef database update

# Rollback last migration
dotnet ef migrations remove

# See all migrations
dotnet ef migrations list
```

---

## 📡 API Endpoints

### Products
```
GET    /api/products              # Get all products
GET    /api/products/{id}         # Get product by ID
GET    /api/products/search?q=    # Search products
POST   /api/products              # Create (Admin only)
PUT    /api/products/{id}         # Update (Admin only)
DELETE /api/products/{id}         # Delete (Admin only)
```

### Authentication
```
POST   /api/auth/register         # Register new user
POST   /api/auth/login            # Login & get JWT token
POST   /api/auth/refresh-token    # Refresh expired token
POST   /api/auth/logout           # Logout
```

### Orders
```
GET    /api/orders                # User's orders
GET    /api/orders/{id}           # Order details
POST   /api/orders                # Create order
PUT    /api/orders/{id}/status    # Update status (Admin)
DELETE /api/orders/{id}           # Cancel order
```

### Cart
```
GET    /api/cart                  # Get user's cart
POST   /api/cart/items            # Add to cart
PUT    /api/cart/items/{id}       # Update cart item
DELETE /api/cart/items/{id}       # Remove from cart
DELETE /api/cart                  # Clear cart
```

### Payments
```
POST   /api/payments              # Process payment
GET    /api/payments/{id}         # Get payment status
POST   /api/payments/{id}/verify  # Verify payment
```

---

## 🧪 Testing

### Run Tests

```bash
dotnet test
dotnet test --verbosity=detailed
dotnet test /p:CollectCoverage=true
```

### Unit Test Example

```csharp
using Xunit;
using Moq;

public class ProductServiceTests
{
    private readonly Mock<ShoexDbContext> _mockContext;
    private readonly ProductService _productService;
    
    public ProductServiceTests()
    {
        _mockContext = new Mock<ShoexDbContext>();
        _mockContext.Setup(c => c.Products).Returns(
            GetMockDbSet(new List<Product>
            {
                new Product { Id = 1, Name = "Nike Air", Price = 100 }
            })
        );
        
        _productService = new ProductService(_mockContext.Object);
    }
    
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        int productId = 1;
        
        // Act
        var result = await _productService.GetByIdAsync(productId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Nike Air", result.Name);
        Assert.Equal(100, result.Price);
    }
    
    [Fact]
    public async Task CreateAsync_WithValidData_CreatesProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Shoe",
            Price = 150,
            Stock = 10
        };
        
        // Act
        var result = await _productService.CreateAsync(dto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Shoe", result.Name);
    }
}
```

---

### Appsettings for Production

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your production connection string"
  },
  "Jwt": {
    "SecretKey": "Your strong secret key (32+ chars)",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*.yourdomain.com"
}
```

---

## 🐛 Troubleshooting

### Database Connection Error
```
Error: Unable to connect to database
Solution:
1. Verify SQL Server is running
2. Check connection string in appsettings.json
3. Run: dotnet ef database update
```

### Migration Issues
```
Error: Migration 'CreateTable' is not applied
Solution:
1. dotnet ef migrations add CreateTable
2. dotnet ef database update
```

### JWT Validation Error
```
Error: 401 Unauthorized
Solution:
1. Check token in Authorization header
2. Verify SecretKey matches in appsettings.json
3. Ensure token not expired
```

---

## 📚 Learning Resources

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [JWT Authentication](https://tools.ietf.org/html/rfc7519)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

## 📞 Contact

- **LinkedIn:** [Ajnas Thayyil](https://www.linkedin.com/in/ajnasthaayyil/)
- **Email:** [ajnasthayyil123@gmail.com](mailto:ajnasthayyil123@gmail.com)
- **WhatsApp:** [+91 7025882784](https://wa.me/917025882784)
- **Portfolio:** [ajnasthayyil.github.io/myportfolio](https://ajnasthayyil.github.io/myportfolio/)

---

<p align="center">
  <strong>Made with ❤️ by Ajnas Thayyil</strong>
  <br/>
  <a href="https://github.com/Ajnasthayyil">Follow on GitHub</a>
</p>

**Last Updated:** April 2026
