using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class AccountCommandHandler :
IRequestHandler<CreateAccountCommand, ApiResponse<AccountResponse>>,
IRequestHandler<UpdateAccountCommand, ApiResponse>,
IRequestHandler<DeleteAccountCommand, ApiResponse>
{
    private readonly OtsDbContext dbContext;
    private readonly IMapper mapper;

    public AccountCommandHandler(OtsDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        // check if account exists
        var entity = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("Account not found");

        if (!entity.IsActive)
            return new ApiResponse("Account is not active");

        // soft delete
        entity.IsActive = false;
        entity.CloseDate = DateTime.Now;
        entity.UpdatedDate = DateTime.Now;
        entity.UpdatedUser = null;

        // update record
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<Account>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("Account not found");

        if (!entity.IsActive)
            return new ApiResponse("Account is not active");

        entity.Name = request.Account.Name;
        entity.UpdatedDate = DateTime.Now;
        entity.UpdatedUser = null;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Account>(request.Account);
        mapped.InsertedDate = DateTime.Now;
        mapped.InsertedUser = "test";
        mapped.IsActive = true;
        mapped.AccountNumber = new Random().Next(100000000, 999999999);
        mapped.IBAN = "TR" + mapped.AccountNumber.ToString("D20");
        mapped.Balance = 0;
        mapped.OpenDate = DateTime.Now;
        
        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var response = mapper.Map<AccountResponse>(entity.Entity);

        return new ApiResponse<AccountResponse>(response);
    }
}
