using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context) 
        {
            // check if Users table contains any Users
            if (await context.Users.AnyAsync()) return;
            // if we continue, that means we do not have any users in our DB
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            // deserealize what's inside the userData here
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            // At this stage, our users should be a normal list of users of type AppUser.
            // We should have it out of the JSON file by this point.
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;
                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}