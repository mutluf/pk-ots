using Microsoft.AspNetCore.Http;

namespace Ots.Base;

public interface IAppSession
{
    public string UserName { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public string UserRole { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public HttpContext HttpContext { get; set; }
}
