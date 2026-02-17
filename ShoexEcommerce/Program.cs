//using CloudinaryDotNet;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using ShoexEcommerce.Application.Common;
//using ShoexEcommerce.Application.Interfaces.Address;
//using ShoexEcommerce.Application.Interfaces.Auth;
//using ShoexEcommerce.Application.Interfaces.Brand;
//using ShoexEcommerce.Application.Interfaces.Cart;
//using ShoexEcommerce.Application.Interfaces.Gender;
//using ShoexEcommerce.Application.Interfaces.Media;
//using ShoexEcommerce.Application.Interfaces.Order;
//using ShoexEcommerce.Application.Interfaces.Product;
//using ShoexEcommerce.Application.Interfaces.User;
//using ShoexEcommerce.Application.Interfaces.Wishlist;
//using ShoexEcommerce.Infrastructure.Data;
//using ShoexEcommerce.Infrastructure.Security;
//using ShoexEcommerce.Infrastructure.Services;
//using ShoexEcommerce.Infrastructure.Settings;
//using System.Security.Claims;
//using System.Text;
//using System.Text.Json;




//var builder = WebApplication.CreateBuilder(args);


//// DbContext

//builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//// Services 
//builder.Services.AddScoped<TokenService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IBrandService, BrandService>();
//builder.Services.AddScoped<IGenderService, GenderService>();
//builder.Services.AddScoped<ISizeService, SizeService>();
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
//builder.Services.AddScoped<ICartService, CartService>();
//builder.Services.AddScoped<IWishlistService, WishlistService>();
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
//builder.Services.AddScoped<IEmailService, EmailService>(); 
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IAddressService, AddressService>();
//builder.Services.AddScoped<IAdminUserService, AdminUserService>();





//// Cloudinary

//builder.Services.AddSingleton(sp =>
//{
//    var config = builder.Configuration.GetSection("Cloudinary");

//    var account = new Account(
//        config["CloudName"],
//        config["ApiKey"],
//        config["ApiSecret"]
//    );

//    return new Cloudinary(account);
//});


//// JWT

//var jwtKey = builder.Configuration["Jwt:Key"];
//if (string.IsNullOrWhiteSpace(jwtKey))
//    throw new Exception("Jwt:Key is missing in ShoexEcommerce.API appsettings.json");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//            RoleClaimType = ClaimTypes.Role,


//            NameClaimType = ClaimTypes.NameIdentifier
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                // Cookie token
//                var token = context.Request.Cookies["access_token"];

//                // Authorization header token
//                if (string.IsNullOrWhiteSpace(token))
//                {
//                    var authHeader = context.Request.Headers["Authorization"].ToString();
//                    if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
//                    {
//                        token = authHeader.Substring("Bearer ".Length).Trim();
//                    }
//                }

//                context.Token = token;
//                return Task.CompletedTask;
//            },

//            OnChallenge = context =>
//            {
//                context.HandleResponse();

//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                context.Response.ContentType = "application/json";

//                var payload = JsonSerializer.Serialize(
//                    ApiResponse<string>.Fail("Login required", 401)
//                );

//                return context.Response.WriteAsync(payload);
//            },

//            OnForbidden = context =>
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                context.Response.ContentType = "application/json";

//                var payload = JsonSerializer.Serialize(
//                    ApiResponse<string>.Fail("Access denied", 403)
//                );

//                return context.Response.WriteAsync(payload);
//            }
//        };
//    });

//builder.Services.AddAuthorization();


//// Controllers + Swagger

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoexEcommerce.API", Version = "v1" });


//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter: Bearer {your token}"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();


//// Dev Middleware + Seeding

//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();

//    app.UseSwagger();
//    app.UseSwaggerUI();

//    using var scope = app.Services.CreateScope();
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    try
//    {
//        await DbSeeder.SeedAsync(db);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("SEED ERROR (non-fatal):");
//        Console.WriteLine(ex.ToString());
//    }
//}


//// Middleware Order

//app.UseHttpsRedirection();

//app.UseRouting();

//app.UseAuthentication();
//app.UseMiddleware<BlockedUserMiddleware>();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();



using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.Interfaces.Address;
using ShoexEcommerce.Application.Interfaces.Auth;
using ShoexEcommerce.Application.Interfaces.Brand;
using ShoexEcommerce.Application.Interfaces.Cart;
using ShoexEcommerce.Application.Interfaces.Gender;
using ShoexEcommerce.Application.Interfaces.Media;
using ShoexEcommerce.Application.Interfaces.Order;
using ShoexEcommerce.Application.Interfaces.Product;
using ShoexEcommerce.Application.Interfaces.User;
using ShoexEcommerce.Application.Interfaces.Wishlist;
using ShoexEcommerce.Infrastructure.Data;
using ShoexEcommerce.Infrastructure.Security;
using ShoexEcommerce.Infrastructure.Services;
using ShoexEcommerce.Infrastructure.Settings;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// If any service reads HttpContext (claims/cookies)
builder.Services.AddHttpContextAccessor();

// Services

builder.Services.AddScoped<TokenService>();              
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

// Cloudinary
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration.GetSection("Cloudinary");

    var account = new Account(
        config["CloudName"],
        config["ApiKey"],
        config["ApiSecret"]
    );

    return new Cloudinary(account);
});

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("Jwt:Key is missing in ShoexEcommerce.API appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Cookie token
                var token = context.Request.Cookies["access_token"];

                // Authorization header token
                if (string.IsNullOrWhiteSpace(token))
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                context.Token = token;
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(
                    ApiResponse<string>.Fail("Login required", 401)
                );

                return context.Response.WriteAsync(payload);
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(
                    ApiResponse<string>.Fail("Access denied", 403)
                );

                return context.Response.WriteAsync(payload);
            }
        };
    });

builder.Services.AddAuthorization();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoexEcommerce.API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Dev + Seeding
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        await DbSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine("SEED ERROR (non-fatal):");
        Console.WriteLine(ex.ToString());
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<BlockedUserMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();