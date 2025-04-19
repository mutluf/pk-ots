using Ots.Api.Domain;
using Ots.Api.Impl.GenericRepository;

namespace Ots.Api.Impl;

public interface IUnitOfWork : IDisposable
{
    Task Complete();
    IGenericRepository<Customer> CustomerRepository { get; }
    IGenericRepository<CustomerPhone> CustomerPhoneRepository { get; }
    IGenericRepository<CustomerAddress> CustomerAddressRepository { get; }
    IGenericRepository<Account> AccountRepository { get; }
    IGenericRepository<AccountTransaction> AccountTransactionRepository { get; }
    IGenericRepository<EftTransaction> EftTransactionRepository { get; }
    IGenericRepository<User> UserRepository { get; }
    IGenericRepository<Country> CountryRepository { get; }
}
