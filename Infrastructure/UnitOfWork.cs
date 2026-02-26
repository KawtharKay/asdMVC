using Application;
using System;
using System.Collections.Generic;
using System.Text;

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
