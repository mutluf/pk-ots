using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MoneyTransfersController : ControllerBase
{
    private readonly IMediator mediator;
    public MoneyTransfersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetByParameters")]
    [Authorize(Roles = "admin,user")]
    public async Task<ApiResponse<List<MoneyTransferResponse>>> GetByParameters()
    {
        var operation = new GetMoneyTransferByParametersQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "admin,user")]
    public async Task<ApiResponse<MoneyTransferResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetMoneyTransferByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    [Authorize(Roles = "user")]
    public async Task<ApiResponse<TransactionResponse>> Post([FromBody] MoneyTransferRequest MoneyTransfer)
    {
        var operation = new CreateMoneyTransferCommand(MoneyTransfer);
        var result = await mediator.Send(operation);
        return result;
    }

}
