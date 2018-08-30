using System.Threading.Tasks;

namespace FinderApp.API.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FinderDbContext context;
        public UnitOfWork(FinderDbContext context)
        {
            this.context = context;

        }
        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}