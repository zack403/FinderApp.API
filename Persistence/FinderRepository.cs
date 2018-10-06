using System.Collections.Generic;
using System.Linq;
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

        public Task<Photo> GetPhoto(int id)
        {
            var photo =  context.Photos.FirstOrDefaultAsync(p => p.Id == id );
            return photo;
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

        public async Task<bool> CompleteAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetIsMainPhotoForUser(int userId)
        {

            return await context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }
    }
}