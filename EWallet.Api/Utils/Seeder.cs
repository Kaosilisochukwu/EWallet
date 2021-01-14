using EWallet.Api.Data;
using EWallet.Api.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace EWallet.Api.Utils
{
    public static class Seeder
    {
        public static void SeedData(IApplicationBuilder builder)
        {
            using(var serviceScope = builder.ApplicationServices.CreateScope())
            {
                //Seed(serviceScope.ServiceProvider.GetService<UserManager<User>());
            }
        }

        public static void Seed(AppDbContext context)
        {
            if (!context.Users.Any())
            {

            }
        }
    }
}
