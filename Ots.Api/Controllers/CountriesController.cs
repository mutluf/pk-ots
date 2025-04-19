using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly IMediator mediator;
    public CountriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }


    [HttpGet("Get/Memory")]
    public async Task<ApiResponse<List<CountryResponse>>> GetAllFromMemory()
    {
        var operation = new GetAllCountryQuery("Memory");
        var result = await mediator.Send(operation);
        return result;
    }


    [HttpGet("Get/Redis")]
    public async Task<ApiResponse<List<CountryResponse>>> GetAllFromRedis()
    {
        var operation = new GetAllCountryQuery("Redis");
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    [ResponseCache(CacheProfileName = "Default45")]
    public async Task<ApiResponse<CountryResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetCountryByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<CountryResponse>> Post([FromBody] CountryRequest Country)
    {
        var operation = new CreateCountryCommand(Country);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] CountryRequest Country)
    {
        var operation = new UpdateCountryCommand(id, Country);
        var result = await mediator.Send(operation);
        return result;
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteCountryCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }

}
