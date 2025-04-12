using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Microsoft.AspNetCore.Authorization;
using Ots.Schema;

namespace Ots.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator mediator;
    public UsersController(IMediator mediator)
    {
        this.mediator = mediator;
    }


    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<UserResponse>>> GetAll()
    {
        var operation = new GetAllUsersQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<UserResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetUserByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<UserResponse>> Post([FromBody] UserRequest User)
    {
        var operation = new CreateUserCommand(User);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] UserRequest User)
    {
        var operation = new UpdateUserCommand(id,User);
        var result = await mediator.Send(operation);
        return result;
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteUserCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }

}
