using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
