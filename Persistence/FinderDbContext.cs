using FinderApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace FinderApp.API.Persistence
{
    public class FinderDbContext : DbContext
    {
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }



        public FinderDbContext(DbContextOptions<FinderDbContext> options)
        : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<User>().Property(x => x.Username).IsRequired();
            modelbuilder.Entity<User>().Property(u => u.PasswordHash).IsRequired();

            modelbuilder.Entity<Value>().HasData(
                new { Id = 1, Name = "value 1" },
                new { Id = 2, Name = "Value 2" },
                new { Id = 3, Name = "Value 3" }
         );
        }
    }
}