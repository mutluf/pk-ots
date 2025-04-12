using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Api.Impl.Service;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Command;

public class EftTransactionsCommandHandler : IRequestHandler<CreateEftTransactionCommand, ApiResponse<TransactionResponse>>
{
    private readonly OtsDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAccountService accountService;

    public EftTransactionsCommandHandler(OtsDbContext dbContext, IMapper mapper, IAccountService accountService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.accountService = accountService;
    }
    public async Task<ApiResponse<TransactionResponse>> Handle(CreateEftTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == request.EftTransaction.AccountId, cancellationToken);
        if (account == null)
            return new ApiResponse<TransactionResponse>("Account not found. Please contact support.");

        if (!account.IsActive)
            return new ApiResponse<TransactionResponse>("Account is not active. Please contact support.");

        var feeAmount = request.EftTransaction.Amount * 0.04m; // 4% fee
        var refNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

        if (account.Balance < (request.EftTransaction.Amount + feeAmount))
            return new ApiResponse<TransactionResponse>("Account balance is not enough");

        var eftTransaction = new EftTransaction
        {
            AccountId = request.EftTransaction.AccountId,
            ReveiverIban = request.EftTransaction.ReveiverIban,
            ReceiverName = request.EftTransaction.ReceiverName,
            Description = request.EftTransaction.Description,
            Amount = request.EftTransaction.Amount,
            PaymentCategoryCode = request.EftTransaction.PaymentCategoryCode,
            TransactionDate = DateTime.Now,
            ReferenceNumber = refNumber,
            FeeAmount = feeAmount
        };

        await dbContext.Set<EftTransaction>().AddAsync(eftTransaction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var responseAccount = await accountService.CreateOutgoingAccountTransaction(account.Id, request.EftTransaction.Amount, feeAmount, request.EftTransaction.Description, refNumber);
        if (responseAccount.Success == false)
            return new ApiResponse<TransactionResponse>(responseAccount.Message);

        var transactionResponse = new TransactionResponse
        {
            Amount = eftTransaction.Amount,
            FeeAmount = feeAmount,
            TransactionDate = eftTransaction.TransactionDate,
            ReferenceNumber = eftTransaction.ReferenceNumber,
        };
        return new ApiResponse<TransactionResponse>(transactionResponse);
    }

}


