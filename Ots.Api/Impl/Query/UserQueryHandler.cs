using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class UserQueryHandler :
IRequestHandler<GetAllUsersQuery, ApiResponse<List<UserResponse>>>,
IRequestHandler<GetUserByIdQuery, ApiResponse<UserResponse>>
{
    private readonly OtsDbContext context;
    private readonly IMapper mapper;
    public UserQueryHandler(OtsDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }
    public async Task<ApiResponse<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var Users = await context.Set<User>().ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<UserResponse>>(Users);
        return new ApiResponse<List<UserResponse>>(mapped);
    }

    public async Task<ApiResponse<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<User>(true);
        predicate = predicate.And(x => x.Id == request.Id);

        var User = await context.Set<User>().FirstOrDefaultAsync(predicate, cancellationToken);

        var mapped = mapper.Map<UserResponse>(User);
        return new ApiResponse<UserResponse>(mapped);
    }
}
