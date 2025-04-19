using MediatR;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Cqrs;

public record GetAllCustomersQuery : IRequest<ApiResponse<List<CustomerResponse>>>;
public record GetCustomerByIdQuery(int Id) : IRequest<ApiResponse<CustomerResponse>>;
public record GetCustomerByParametersQuery(string? FirstName, string? LastName, string? Email) : IRequest<ApiResponse<List<CustomerResponse>>>;
public record CreateCustomerCommand(CustomerRequest customer) : IRequest<ApiResponse<CustomerResponse>>;
public record UpdateCustomerCommand(int Id, CustomerRequest customer) : IRequest<ApiResponse>;
public record DeleteCustomerCommand(int Id) : IRequest<ApiResponse>;