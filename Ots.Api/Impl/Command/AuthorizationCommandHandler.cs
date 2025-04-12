using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Api.Impl.Service;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class AuthorizationCommandHandler :
IRequestHandler<CreateAuthorizationTokenCommand, ApiResponse<AuthorizationResponse>>
{
    private readonly OtsDbContext dbContext;
    private readonly ITokenService tokenService;
    private readonly JwtConfig jwtConfig;

    public AuthorizationCommandHandler(OtsDbContext dbContext, ITokenService tokenService, JwtConfig jwtConfig)
    {
        this.jwtConfig = jwtConfig;
        this.dbContext = dbContext;
        this.tokenService = tokenService;
    }
    public async Task<ApiResponse<AuthorizationResponse>> Handle(CreateAuthorizationTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>().FirstOrDefaultAsync(x => x.UserName == request.Request.UserName, cancellationToken);
        if (user == null)
            return new ApiResponse<AuthorizationResponse>("User name or password is incorrect");

        var hashedPassword = PasswordGenerator.CreateMD5(request.Request.Password, user.Secret);
        if (hashedPassword != user.Password)
            return new ApiResponse<AuthorizationResponse>("User name or password is incorrect");

        var token = tokenService.GenerateToken(user);
        var entity = new AuthorizationResponse
        {
            UserName = user.UserName,
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(jwtConfig.AccessTokenExpiration)
        };

        return new ApiResponse<AuthorizationResponse>(entity);
    }
}
