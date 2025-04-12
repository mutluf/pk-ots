using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class CustomerPhoneCommandHandler :
IRequestHandler<CreateCustomerPhoneCommand, ApiResponse<CustomerPhoneResponse>>,
IRequestHandler<UpdateCustomerPhoneCommand, ApiResponse>,
IRequestHandler<DeleteCustomerPhoneCommand, ApiResponse>
{
    private readonly OtsDbContext dbContext;
    private readonly IMapper mapper;

    public CustomerPhoneCommandHandler(OtsDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteCustomerPhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<CustomerPhone>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("CustomerPhone not found");

        if (!entity.IsActive)
            return new ApiResponse("CustomerPhone is not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateCustomerPhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<CustomerPhone>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            return new ApiResponse("CustomerPhone not found");

        if (!entity.IsActive)
            return new ApiResponse("CustomerPhone is not active");

        entity.PhoneNumber = request.CustomerPhone.PhoneNumber;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse<CustomerPhoneResponse>> Handle(CreateCustomerPhoneCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<CustomerPhone>(request.CustomerPhone);
        mapped.IsActive = true;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var response = mapper.Map<CustomerPhoneResponse>(entity.Entity);

        return new ApiResponse<CustomerPhoneResponse>(response);
    }
}
