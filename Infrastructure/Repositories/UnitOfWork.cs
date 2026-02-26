using Application.Repositories;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public async Task<int> SaveAsync()
        {
            return await SaveAsync();
        }
    }
}
