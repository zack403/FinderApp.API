using System.Threading.Tasks;

namespace FinderApp.API.Persistence
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}