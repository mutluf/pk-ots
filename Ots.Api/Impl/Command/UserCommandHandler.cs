using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class UserCommandHandler :
IRequestHandler<CreateUserCommand, ApiResponse<UserResponse>>,
IRequestHandler<UpdateUserCommand, ApiResponse>,
IRequestHandler<DeleteUserCommand, ApiResponse>
{
    private readonly OtsDbContext dbContext;
    private readonly IMapper mapper;

    public UserCommandHandler(OtsDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("User not found");

        if (!entity.IsActive)
            return new ApiResponse("User is not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("User not found");

        if (!entity.IsActive)
            return new ApiResponse("User is not active");

        entity.FirstName = request.User.FirstName;
        entity.LastName = request.User.LastName;
        entity.Role = request.User.Role;        
        entity.UpdatedDate = DateTime.Now;
        entity.UpdatedUser = null;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<User>(request.User);
        mapped.InsertedDate = DateTime.Now;
        mapped.OpenDate = DateTime.Now;
        mapped.InsertedUser = "test";
        mapped.IsActive = true;
        mapped.Secret = PasswordGenerator.GeneratePassword(30);

        var password = PasswordGenerator.GeneratePassword(6);
        mapped.Password = PasswordGenerator.CreateMD5(password, mapped.Secret);

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var response = mapper.Map<UserResponse>(entity.Entity);

        return new ApiResponse<UserResponse>(response);
    }
}
