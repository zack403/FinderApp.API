using System.Collections.Generic;
using FinderApp.API.Model;
using Newtonsoft.Json;

namespace FinderApp.API.Persistence
{
    public class Seed
    {
        private readonly FinderDbContext context;
        public Seed(FinderDbContext context)
        {
            this.context = context;
            
        }
        public void SeedUsers()
        {
            //removes existing users from the database
            //context.Users.RemoveRange(context.Users);
            //context.SaveChanges();

            //seed or create new users to the database

            var userData = System.IO.File.ReadAllText("Persistence/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach(var user in users)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                context.Users.Add(user);
            }
                context.SaveChanges();

        }


              private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
    }
}