using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;
using Microsoft.AspNetCore.Authorization;

namespace Ots.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EftTransactionsController : ControllerBase
{
    private readonly IMediator mediator;

    public EftTransactionsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetByParameters")]
    public async Task<ApiResponse<List<EftTransactionResponse>>> GetByParameters()
    {
        var operation = new GetEftTransactionByParametersQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<EftTransactionResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetEftTransactionByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<TransactionResponse>> Post([FromBody] EftTransactionRequest EftTransaction)
    {
        var operation = new CreateEftTransactionCommand(EftTransaction);
        var result = await mediator.Send(operation);
        return result;
    }

}
