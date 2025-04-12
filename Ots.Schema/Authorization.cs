using Ots.Base;

namespace Ots.Schema;

public class AuthorizationRequest : BaseRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class AuthorizationResponse 
{
    public string Token { get; set; }
    public string UserName { get; set; }
    public DateTime Expiration { get; set; }
}