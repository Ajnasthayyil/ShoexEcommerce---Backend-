using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShoexEcommerce.Domain.Entities;

namespace ShoexEcommerce.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration config)
        {
            await context.Database.MigrateAsync();

         
            //Seed Roles
        
            if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
                context.Roles.Add(new Role { Name = "Admin" });

            if (!await context.Roles.AnyAsync(r => r.Name == "User"))
                context.Roles.Add(new Role { Name = "User" });

            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

           
            //Seed Admin User
       
            var adminUsername = config["Seed:AdminUsername"] ?? "admin123";
            var adminEmail = config["Seed:AdminEmail"] ?? "admin123@gmail.com";
            var adminMobile = config["Seed:AdminMobile"] ?? "9878961243";
            var adminPassword = config["Seed:AdminPassword"] ?? "Admin@123";

            var adminExists = await context.Users.AnyAsync(u =>
                u.Username == adminUsername ||
                u.Email == adminEmail ||
                u.MobileNumber == adminMobile);

            if (!adminExists)
            {
                context.Users.Add(new User
                {
                    FullName = "System Administrator",
                    Username = adminUsername,
                    Email = adminEmail,
                    MobileNumber = adminMobile,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    RoleId = adminRole.Id,
                    IsActive = true
                });

                await context.SaveChangesAsync();
            }

           
            //Seed Gender

            if (!await context.Genders.AnyAsync())
            {
                context.Genders.AddRange(
                    new Gender { Name = "Male", IsActive = true },
                    new Gender { Name = "Female", IsActive = true }
                );

                await context.SaveChangesAsync();
            }

            //Size seeding

            if (!await context.Sizes.AnyAsync())
            {
                context.Sizes.AddRange(
                    new Size { Name = "UK 6", IsActive = true },
                    new Size { Name = "UK 7", IsActive = true },
                    new Size { Name = "UK 8", IsActive = true },
                    new Size { Name = "UK 9", IsActive = true },
                    new Size { Name = "UK 10", IsActive = true }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
