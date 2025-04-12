using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class CustomerAddressCommandHandler :
IRequestHandler<CreateCustomerAddressCommand, ApiResponse<CustomerAddressResponse>>,
IRequestHandler<UpdateCustomerAddressCommand, ApiResponse>,
IRequestHandler<DeleteCustomerAddressCommand, ApiResponse>
{
    private readonly OtsDbContext dbContext;
    private readonly IMapper mapper;

    public CustomerAddressCommandHandler(OtsDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<CustomerAddress>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("CustomerAddress not found");

        if (!entity.IsActive)
            return new ApiResponse("CustomerAddress is not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<CustomerAddress>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("CustomerAddress not found");

        if (!entity.IsActive)
            return new ApiResponse("CustomerAddress is not active");

        entity.City = request.CustomerAddress.City;
        entity.District = request.CustomerAddress.District;
        entity.Street = request.CustomerAddress.Street;
        entity.ZipCode = request.CustomerAddress.ZipCode;
        entity.CountryCode = request.CustomerAddress.CountryCode;
        entity.IsDefault = request.CustomerAddress.IsDefault;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse<CustomerAddressResponse>> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<CustomerAddress>(request.CustomerAddress);
        mapped.IsActive = true;
        mapped.CustomerId = request.CustomerId;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var response = mapper.Map<CustomerAddressResponse>(entity.Entity);

        return new ApiResponse<CustomerAddressResponse>(response);
    }
}
