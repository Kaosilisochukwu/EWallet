using EWallet.Api.Data;
using EWallet.Api.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Utils
{
    public static class Seeder
    {
        public static async Task SeedData(UserManager<User> userManager, AppDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    FirstName = "James",
                    LastName = "Badmus",
                    Role = "Admin",
                    UserName = "Badjames",
                    Email = "badjames@muse.com",
                    DateAdded = DateTime.Now
                };
                await userManager.CreateAsync(admin, "01234Admin");
            }
        }
        
    }
}
