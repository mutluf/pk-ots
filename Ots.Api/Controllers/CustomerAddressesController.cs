using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;
using Microsoft.AspNetCore.Authorization;

namespace Ots.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerAddresssController : ControllerBase
{
    private readonly IMediator mediator;
    public CustomerAddresssController(IMediator mediator)
    {
        this.mediator = mediator;
    }


    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<CustomerAddressResponse>>> GetAll()
    {
        var operation = new GetAllCustomerAddressQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<CustomerAddressResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetCustomerAddressByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("ByCustomerId/{CustomerId}")]
    public async Task<ApiResponse<CustomerAddressResponse>> Post(int CustomerId, [FromBody] CustomerAddressRequest CustomerAddress)
    {
        var operation = new CreateCustomerAddressCommand(CustomerId,CustomerAddress);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] CustomerAddressRequest CustomerAddress)
    {
        var operation = new UpdateCustomerAddressCommand(id,CustomerAddress);
        var result = await mediator.Send(operation);
        return result;
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteCustomerAddressCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }

}
