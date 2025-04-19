using Ots.Api.Domain;
using Ots.Api.Impl.GenericRepository;

namespace Ots.Api.Impl;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly OtsDbContext dbContext;

    public UnitOfWork(OtsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IGenericRepository<Customer> CustomerRepository => new GenericRepository<Customer>(dbContext);
    public IGenericRepository<CustomerPhone> CustomerPhoneRepository => new GenericRepository<CustomerPhone>(dbContext);
    public IGenericRepository<CustomerAddress> CustomerAddressRepository => new GenericRepository<CustomerAddress>(dbContext);
    public IGenericRepository<Account> AccountRepository => new GenericRepository<Account>(dbContext);
    public IGenericRepository<AccountTransaction> AccountTransactionRepository => new GenericRepository<AccountTransaction>(dbContext);
    public IGenericRepository<EftTransaction> EftTransactionRepository => new GenericRepository<EftTransaction>(dbContext);
    public IGenericRepository<User> UserRepository => new GenericRepository<User>(dbContext);

    public async Task Complete()
    {
        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
    public void Dispose()
    {
        dbContext.Dispose();
    }


}
