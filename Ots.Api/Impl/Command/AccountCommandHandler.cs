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
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public AccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        // check if account exists
        var entity = await unitOfWork.AccountRepository.GetByIdAsync(request.Id);
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
        unitOfWork.AccountRepository.Delete(entity);
        await unitOfWork.Complete();
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.AccountRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return new ApiResponse("Account not found");

        if (!entity.IsActive)
            return new ApiResponse("Account is not active");

        entity.Name = request.Account.Name;

        unitOfWork.AccountRepository.Update(entity);
        await unitOfWork.Complete();
        return new ApiResponse();
    }

    public async Task<ApiResponse<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Account>(request.Account);
        mapped.IsActive = true;
        mapped.AccountNumber = new Random().Next(100000000, 999999999);
        mapped.IBAN = "TR" + mapped.AccountNumber.ToString("D20");
        mapped.Balance = 0;
        mapped.OpenDate = DateTime.Now;

        var entity = await unitOfWork.AccountRepository.AddAsync(mapped);
        await unitOfWork.Complete();
        var response = mapper.Map<AccountResponse>(entity);
        return new ApiResponse<AccountResponse>(response);
    }
}
