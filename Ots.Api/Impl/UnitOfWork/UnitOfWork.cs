using Ots.Api.Domain;
using Ots.Api.Impl.GenericRepository;
using Serilog;

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
                Log.Error(ex, "Error occurred while saving changes to the database.");
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private bool _disposed = false;

}
