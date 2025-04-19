using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator mediator;
    public AccountsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    [Authorize(Roles = "admin,user")]
    public async Task<ApiResponse<List<AccountResponse>>> GetAll()
    {
        var operation = new GetAllAccountsQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "admin,user")]
    public async Task<ApiResponse<AccountResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetAccountByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    [Authorize(Roles = "admin,user")]
    public async Task<ApiResponse<AccountResponse>> Post([FromBody] AccountRequest Account)
    {
        var operation = new CreateAccountCommand(Account);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,user")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] AccountRequest Account)
    {
        var operation = new UpdateAccountCommand(id, Account);
        var result = await mediator.Send(operation);
        return result;
    }
    [HttpDelete("{id}")]    
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteAccountCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }

}
