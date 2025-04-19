using AutoMapper;
using MediatR;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class CustomerAddressQueryHandler :
IRequestHandler<GetAllCustomerAddressQuery, ApiResponse<List<CustomerAddressResponse>>>,
IRequestHandler<GetCustomerAddressByIdQuery, ApiResponse<CustomerAddressResponse>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CustomerAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<CustomerAddressResponse>>> Handle(GetAllCustomerAddressQuery request, CancellationToken cancellationToken)
    {
        var customerAddresses = await unitOfWork.CustomerAddressRepository.GetAllAsync("Customer");
        var mapped = mapper.Map<List<CustomerAddressResponse>>(customerAddresses);
        return new ApiResponse<List<CustomerAddressResponse>>(mapped);
    }

    public async Task<ApiResponse<CustomerAddressResponse>> Handle(GetCustomerAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var customerAddress = await unitOfWork.CustomerAddressRepository.GetByIdAsync(request.Id, "Customer");
        var mapped = mapper.Map<CustomerAddressResponse>(customerAddress);
        return new ApiResponse<CustomerAddressResponse>(mapped);
    }
}
