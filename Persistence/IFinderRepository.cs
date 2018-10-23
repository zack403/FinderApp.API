using System.Collections.Generic;
using System.Threading.Tasks;
using FinderApp.API.Helpers;
using FinderApp.API.Model;

namespace FinderApp.API.Persistence
{
    public interface IFinderRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> CompleteAsync();

         Task<PagedList<User>> GetUsers(UserParams userparams);
         Task<User> GetUser(int id);

         Task<Photo> GetPhoto(int id);

         Task<Photo> GetIsMainPhotoForUser(int userId);

         Task<Like> GetLike(int userId, int recipientId);
         Task<Message> GetMessage(int id);
         Task<PagedList<Message>> GetMessagesForUser(MessageParams messageparams);
         Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);


    }
}