using AutoMapper;
using MediatR;
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
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CustomerPhoneCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteCustomerPhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.CustomerPhoneRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return new ApiResponse("CustomerPhone not found");

        if (!entity.IsActive)
            return new ApiResponse("CustomerPhone is not active");

        entity.IsActive = false;

        unitOfWork.CustomerPhoneRepository.Update(entity);
        await unitOfWork.Complete();
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateCustomerPhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.CustomerPhoneRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return new ApiResponse("CustomerPhone not found");

        if (!entity.IsActive)
            return new ApiResponse("CustomerPhone is not active");

        entity.PhoneNumber = request.CustomerPhone.PhoneNumber;

        unitOfWork.CustomerPhoneRepository.Update(entity);
        await unitOfWork.Complete();
        return new ApiResponse();
    }

    public async Task<ApiResponse<CustomerPhoneResponse>> Handle(CreateCustomerPhoneCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<CustomerPhone>(request.CustomerPhone);
        mapped.IsActive = true;

        var entity = await unitOfWork.CustomerPhoneRepository.AddAsync(mapped);
        await unitOfWork.Complete();
        var response = mapper.Map<CustomerPhoneResponse>(entity);

        return new ApiResponse<CustomerPhoneResponse>(response);
    }
}
