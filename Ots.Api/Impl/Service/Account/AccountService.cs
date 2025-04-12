using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Base;

namespace Ots.Api.Impl.Service;

public class AccountService : IAccountService
{
    private readonly OtsDbContext dbContext;
    public AccountService(OtsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Creates an incoming account transaction, adding the specified amount to the account balance.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="amount">The amount of the transaction. Must be greater than zero.</param>
    /// <param name="description">The description of the transaction. Must not be empty.</param>
    /// <param name="refNumber">The reference number of the transaction. Must not be empty.</param>
    /// <returns>A response indicating the success or failure of the operation.</returns>
    public async Task<ApiResponse> CreateIncomingAccountTransaction(long accountId, decimal amount, 
    string description, string refNumber)
    {
        if (amount <= 0)
            return new ApiResponse("Amount must be greater than zero");
        if (string.IsNullOrEmpty(description))
            return new ApiResponse("Description cannot be empty");
        if (string.IsNullOrEmpty(refNumber))
            return new ApiResponse("Reference number cannot be empty");

        var account = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == accountId);
        if (account == null)
            return new ApiResponse("Account not found");
        if (!account.IsActive)
            return new ApiResponse("Account is not active");

        var accountTransaction = new AccountTransaction
        {
            AccountId = accountId,
            DebitAmount = amount,
            Description = description,
            TransactionDate = DateTime.Now,
            InsertedDate = DateTime.Now,
            InsertedUser = "System",
            ReferenceNumber = refNumber,
        };

        await dbContext.Set<AccountTransaction>().AddAsync(accountTransaction);

        account.Balance += amount;
        account.UpdatedDate = DateTime.Now;
        account.UpdatedUser = null;

        await dbContext.SaveChangesAsync();
        return new ApiResponse();
    }

    /// <summary>
    /// Creates an outgoing account transaction, subtracting the amount and fee from the account balance.
    /// </summary>
    /// <param name="accountId">The id of the account</param>
    /// <param name="amount">The amount of the transaction. Must be greater than zero.</param>
    /// <param name="feeAmount">The amount of fee. Must be greater than or equal to zero.</param>
    /// <param name="description">The description of the transaction. Must not be empty.</param>
    /// <param name="refNumber">The reference number of the transaction. Must not be empty.</param>
    /// <returns>A response indicating success or failure</returns>
    public async Task<ApiResponse> CreateOutgoingAccountTransaction(long accountId, decimal amount, 
    decimal feeAmount, string description, string refNumber)
    {
        if (amount <= 0)
            return new ApiResponse("Amount must be greater than zero. Please contact support.");
        if (string.IsNullOrEmpty(description))
            return new ApiResponse("Description cannot be empty. Please contact support.");
        if (string.IsNullOrEmpty(refNumber))
            return new ApiResponse("Reference number cannot be empty. Please contact support.");

        var account = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == accountId);
        if (account == null)
            return new ApiResponse("Account not found");
        if (!account.IsActive)
            return new ApiResponse("Account is not active");

        if (account.Balance < (amount + feeAmount))
            return new ApiResponse("Account balance is not enough");

        var accountTransaction = new AccountTransaction
        {
            AccountId = accountId,
            CreditAmount = amount,
            Description = description,
            TransactionDate = DateTime.Now,
            InsertedDate = DateTime.Now,
            InsertedUser = "System",
            ReferenceNumber = refNumber,
        };

        if (feeAmount > 0)
        {
            var accountTransactionFee = new AccountTransaction
            {
                AccountId = accountId,
                CreditAmount = feeAmount,
                Description = description + " - Fee",
                TransactionDate = DateTime.Now,
                InsertedDate = DateTime.Now,
                InsertedUser = "System",
                ReferenceNumber = refNumber,
                TransferType = "Fee",
            };
            await dbContext.Set<AccountTransaction>().AddAsync(accountTransactionFee);
        }
        await dbContext.Set<AccountTransaction>().AddAsync(accountTransaction);

        account.Balance -= (amount + feeAmount);
        account.UpdatedDate = DateTime.Now;
        account.UpdatedUser = null;

        await dbContext.SaveChangesAsync();
        return new ApiResponse();
    }
}
