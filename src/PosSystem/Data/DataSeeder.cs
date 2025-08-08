using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosSystem.Models;

namespace PosSystem.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Store
            var store = await SeedStoreAsync(context);

            // Seed Administrator User
            await SeedAdminUserAsync(userManager, store.Id);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = { "Administrator", "Manager", "Cashier", "Staff" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }

        private static async Task<Store> SeedStoreAsync(ApplicationDbContext context)
        {
            if (!await context.Stores.AnyAsync())
            {
                var store = new Store
                {
                    StoreName = "Main Store",
                    StoreCode = "MAIN001",
                    Address = "123 Main Street, Jakarta, Indonesia",
                    Phone = "+62-21-12345678",
                    TaxRate = 0.11m,
                    Currency = "IDR",
                    Timezone = "Asia/Jakarta",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Stores.Add(store);
                await context.SaveChangesAsync();
                return store;
            }

            return await context.Stores.FirstAsync();
        }

        private static async Task SeedAdminUserAsync(UserManager<User> userManager, int storeId)
        {
            var adminEmail = "admin@possystem.com";
            
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = adminEmail,
                    UserName = adminEmail,
                    PhoneNumber = "+62-21-87654321",
                    Role = "Administrator",
                    StoreId = storeId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    Console.WriteLine("Administrator user created successfully:");
                    Console.WriteLine($"Email: {adminEmail}");
                    Console.WriteLine("Password: Admin123!");
                    Console.WriteLine("Role: Administrator");
                }
                else
                {
                    Console.WriteLine("Failed to create administrator user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Administrator user already exists.");
            }
        }
    }
}