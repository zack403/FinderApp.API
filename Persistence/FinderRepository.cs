using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinderApp.API.Helpers;
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

        public async Task<PagedList<User>> GetUsers(UserParams userparams)
        {
            var users =  context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            // filtering the current user
            users = users.Where(u => u.Id != userparams.userId);

            //filtering by gender
            users = users.Where(g => g.Gender == userparams.Gender);

            if(userparams.Likers){
                var userLikers = await GetUserLikes(userparams.userId, userparams.Likers);
                users = users.Where(u => userLikers.Any(liker => liker.LikerId == u.Id));
            }

            if(userparams.Likees){
                var userLikees = await GetUserLikes(userparams.userId, userparams.Likers);
                users = users.Where(u => userLikees.Any(likee => likee.LikeeId == u.Id));

            }
            
            //filtering by Age

            if(userparams.MinAge != 18 || userparams.MaxAge != 99){
                users = users.Where(u => u.DateOfBirth.CalculateAge() >= userparams.MinAge && u.DateOfBirth.CalculateAge() <= userparams.MaxAge);
            }

            //sorting
            if(!string.IsNullOrEmpty(userparams.OrderBy)){
                switch(userparams.OrderBy)
                {
                    case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                    default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userparams.PageNumber, userparams.PageSize); 
        }


        private async Task<IEnumerable<Like>> GetUserLikes(int id, bool likers)
        {
            var user = await context.Users
            .Include(l => l.Likee)
            .Include(l => l.Liker)
            .FirstOrDefaultAsync(u => u.Id == id);
            if(likers) {
                return user.Liker.Where(u => u.LikeeId == id);
            }else {
                return user.Likee.Where(u => u.LikerId == id);
            }
        }

        public async Task<bool> CompleteAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetIsMainPhotoForUser(int userId)
        {

            return await context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await context.Likes.FirstOrDefaultAsync(l => l.LikerId == userId && l.LikeeId == recipientId);
        }
    }
}