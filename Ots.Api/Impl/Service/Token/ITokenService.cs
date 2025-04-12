using Ots.Api.Domain;

namespace Ots.Api.Impl.Service;

public interface ITokenService
{
    public string GenerateToken(User user);
}
