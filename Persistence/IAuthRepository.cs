using System.Threading.Tasks;
using FinderApp.API.Model;

namespace FinderApp.API.Persistence
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);

    }
}