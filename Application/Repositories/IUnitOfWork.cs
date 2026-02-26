using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
