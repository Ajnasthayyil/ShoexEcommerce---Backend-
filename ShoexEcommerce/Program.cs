using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.Interfaces;
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

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));

#endregion

#region Services

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<IShoppingAssistantService, ShoppingAssistantService>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddHttpClient();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

#endregion

#region Cloudinary

builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration.GetSection("Cloudinary");

    var account = new Account(
        config["CloudName"],
        config["ApiKey"],
        config["ApiSecret"]);

    return new Cloudinary(account);
});

#endregion

#region JWT Authentication

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("Jwt:Key is missing in configuration.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];

                if (string.IsNullOrWhiteSpace(token))
                {
                    var authHeader =
                        context.Request.Headers["Authorization"]
                        .ToString();

                    if (!string.IsNullOrWhiteSpace(authHeader) &&
                        authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader["Bearer ".Length..].Trim();
                    }
                }

                context.Token = token;
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(
                    ApiResponse<string>.Fail(
                        "Login required", 401));

                return context.Response.WriteAsync(payload);
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(
                    ApiResponse<string>.Fail(
                        "Access denied", 403));

                return context.Response.WriteAsync(payload);
            }
        };
    });

builder.Services.AddAuthorization();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://shoex-ecommerce.vercel.app/" 
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#endregion

#region Controllers

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Shoex Ecommerce API",
            Version = "v1"
        });

    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Bearer {token}"
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            var errorMessage = exception != null 
                ? $"{exception.GetType().Name}: {exception.Message}\nStack Trace:\n{exception.StackTrace}" 
                : "An unexpected error occurred. Please try again later.";
            
            if (exception?.InnerException != null)
            {
                errorMessage += $"\nInner Exception: {exception.InnerException.GetType().Name}: {exception.InnerException.Message}\nStack Trace:\n{exception.InnerException.StackTrace}";
            }
            
            var payload = JsonSerializer.Serialize(
                ApiResponse<string>.Fail(errorMessage, 500));
            await context.Response.WriteAsync(payload);
        });
    });
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseMiddleware<BlockedUserMiddleware>();

app.UseAuthorization();

#endregion

#region Database Seeding

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    try
    {
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        await DbSeeder.SeedAsync(db, config);
    }
    catch (Exception ex)
    {
        Diagnostics.StartupException = ex;
        Console.WriteLine("SEED ERROR:");
        Console.WriteLine(ex.ToString());
    }
}

#endregion

#region Endpoints

app.MapGet("/", () =>
{
    return Results.Redirect("/swagger");
});

app.MapGet("/db-status", async (AppDbContext db) =>
{
    if (Diagnostics.StartupException != null)
    {
        return Results.Problem(
            detail: Diagnostics.StartupException.ToString(),
            statusCode: 500,
            title: "Database Seeding/Migration Failed");
    }

    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
        var appliedMigrations = await db.Database.GetAppliedMigrationsAsync();

        return Results.Ok(new
        {
            CanConnect = canConnect,
            PendingMigrations = pendingMigrations,
            AppliedMigrations = appliedMigrations
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.ToString(),
            statusCode: 500,
            title: "Database Access Error");
    }
});

app.MapControllers();

#endregion

app.Run();

public static class Diagnostics
{
    public static Exception? StartupException { get; set; }
}