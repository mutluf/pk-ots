using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Api.Impl.Service;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Command;

public class MoneyTransfersCommandHandler : IRequestHandler<CreateMoneyTransferCommand, ApiResponse<TransactionResponse>>
{
    private readonly OtsDbContext dbContext;    
    private readonly IAccountService accountService;

    public MoneyTransfersCommandHandler(OtsDbContext dbContext, IAccountService accountService)
    {
        this.dbContext = dbContext;
        this.accountService = accountService;
    }
    public async Task<ApiResponse<TransactionResponse>> Handle(CreateMoneyTransferCommand request, CancellationToken cancellationToken)
    {
        var fromAccount = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == request.MoneyTransfer.FromAccountId, cancellationToken);
        if (fromAccount == null)
            return new ApiResponse<TransactionResponse>("Account not found");

        if (!fromAccount.IsActive)
            return new ApiResponse<TransactionResponse>("Account is not active");

        var toAccount = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == request.MoneyTransfer.ToAccountId, cancellationToken);
        if (toAccount == null)
            return new ApiResponse<TransactionResponse>("Account not found");

        if (!toAccount.IsActive)
            return new ApiResponse<TransactionResponse>("Account is not active");

        if(fromAccount.CurrencyCode != toAccount.CurrencyCode)
            return new ApiResponse<TransactionResponse>("Currency codes are not same");

        decimal feeAmount = 0;
        if (fromAccount.CustomerId == toAccount.CustomerId)
            feeAmount = request.MoneyTransfer.Amount * 0.01m; // 1% fee        
        else
            feeAmount = request.MoneyTransfer.Amount * 0.02m; // 2% fee       

        var refNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

        if (fromAccount.Balance < (request.MoneyTransfer.Amount + feeAmount))
            return new ApiResponse<TransactionResponse>("Account balance is not enough");

        var MoneyTransfer = new MoneyTransfer
        {
            FromAccountId = request.MoneyTransfer.FromAccountId,
            ToAccountId = request.MoneyTransfer.ToAccountId,
            Description = request.MoneyTransfer.Description,
            Amount = request.MoneyTransfer.Amount,
            TransactionDate = DateTime.Now,
            ReferenceNumber = refNumber,
            FeeAmount = feeAmount,
        };

        await dbContext.Set<MoneyTransfer>().AddAsync(MoneyTransfer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var responseOutgoing = await accountService.CreateOutgoingAccountTransaction(fromAccount.Id, request.MoneyTransfer.Amount, feeAmount, request.MoneyTransfer.Description, refNumber);
        if (responseOutgoing.Success == false)
            return new ApiResponse<TransactionResponse>(responseOutgoing.Message);

        var responseIncoming = await accountService.CreateIncomingAccountTransaction(toAccount.Id, request.MoneyTransfer.Amount, request.MoneyTransfer.Description, refNumber);
        if (responseIncoming.Success == false)
            return new ApiResponse<TransactionResponse>(responseIncoming.Message);

        var transactionResponse = new TransactionResponse
        {
            Amount = MoneyTransfer.Amount,
            FeeAmount = feeAmount,
            TransactionDate = MoneyTransfer.TransactionDate,
            ReferenceNumber = MoneyTransfer.ReferenceNumber,
        };
        return new ApiResponse<TransactionResponse>(transactionResponse);
    }

}


