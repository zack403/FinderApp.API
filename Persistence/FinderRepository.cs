using System.Collections.Generic;
using System.Threading.Tasks;
using FinderApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace FinderApp.API.Persistence
{
    public class FinderRepository : IFinderRepository
    {
        private readonly FinderDbContext context;
        public FinderRepository(FinderDbContext context)
        {
            this.context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
 
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await context.Users.Include(p => p.Photos).ToListAsync();
            return users; 
        }

        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}