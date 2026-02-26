using Application;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public async Task<int> SaveAsync()
        {
            return await SaveAsync();
        }
    }
}
