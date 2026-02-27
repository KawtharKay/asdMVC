using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly AppDbContext _context;

    public BankAccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(BankAccount bankAccount)
    {
        await _context.Set<BankAccount>().AddAsync(bankAccount);
    }

    public async Task<IEnumerable<BankAccount>> GetAllBankAccountsAsync()
    {
        return await _context.Set<BankAccount>().ToListAsync();
    }

    public async Task<BankAccount?> GetBankAccountAsync(Guid customerId)
    {
        return await _context.Set<BankAccount>().Include(x => x.Customer).SingleOrDefaultAsync(x => x.CustomerId == customerId);
    }

    public async Task<IEnumerable<BankAccount>> GetAllBankAccountsByCustomerAsync(Guid customerId)
    {
        return await _context.Set<BankAccount>().Include(x => x.Customer).Where(x => x.CustomerId == customerId).ToListAsync();
    }
}