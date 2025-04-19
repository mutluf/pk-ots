using AutoMapper;
using MediatR;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class AccountQueryHandler :
IRequestHandler<GetAllAccountsQuery, ApiResponse<List<AccountResponse>>>,
IRequestHandler<GetAccountByIdQuery, ApiResponse<AccountResponse>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public AccountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await unitOfWork.AccountRepository.GetAllAsync("Customer");
        var mapped = mapper.Map<List<AccountResponse>>(accounts);
        return new ApiResponse<List<AccountResponse>>(mapped);
    }

    public async Task<ApiResponse<AccountResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await unitOfWork.AccountRepository.GetByIdAsync(request.Id, "Customer");
        var mapped = mapper.Map<AccountResponse>(account);
        return new ApiResponse<AccountResponse>(mapped);
    }
}
