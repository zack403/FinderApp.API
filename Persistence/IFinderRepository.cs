using System.Collections.Generic;
using System.Threading.Tasks;
using FinderApp.API.Model;

namespace FinderApp.API.Persistence
{
    public interface IFinderRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();

         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);

    }
}