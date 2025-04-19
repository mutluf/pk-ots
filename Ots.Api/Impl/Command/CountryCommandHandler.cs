using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Ots.Api.Domain;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class CountryCommandHandler :
IRequestHandler<CreateCountryCommand, ApiResponse<CountryResponse>>,
IRequestHandler<UpdateCountryCommand, ApiResponse>,
IRequestHandler<DeleteCountryCommand, ApiResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDistributedCache distributedCache;
    private readonly IMemoryCache memoryCache;
    private readonly string cacheKey = "CountryList-78";

    public CountryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCache, IMemoryCache memoryCache)
    {
        this.distributedCache = distributedCache;
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.memoryCache = memoryCache;
    }

    public async Task<ApiResponse> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        // check if Country exists
        var entity = await unitOfWork.CountryRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return new ApiResponse("Country not found");

        if (!entity.IsActive)
            return new ApiResponse("Country is not active");

        // soft delete
        entity.IsActive = false;
        entity.UpdatedDate = DateTime.Now;
        entity.UpdatedUser = null;

        // update record
        unitOfWork.CountryRepository.Delete(entity);
        await unitOfWork.Complete();

        await SetRedisCache();
        await SetMemoryCache();
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.CountryRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return new ApiResponse("Country not found");

        if (!entity.IsActive)
            return new ApiResponse("Country is not active");

        entity.Name = request.Country.Name;

        unitOfWork.CountryRepository.Update(entity);
        await unitOfWork.Complete();

        await SetRedisCache();
        await SetMemoryCache();
        return new ApiResponse();
    }

    public async Task<ApiResponse<CountryResponse>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Country>(request.Country);
        mapped.IsActive = true;

        var entity = await unitOfWork.CountryRepository.AddAsync(mapped);
        await unitOfWork.Complete();
        var response = mapper.Map<CountryResponse>(entity);

        await SetRedisCache();
        await SetMemoryCache();
        return new ApiResponse<CountryResponse>(response);
    }

    private Task SetRedisCache()
    {
        distributedCache.Remove(cacheKey);

        var countries = unitOfWork.CountryRepository.GetAllAsync().Result;
        if (countries == null)
            return Task.CompletedTask;

        var mapped = mapper.Map<List<CountryResponse>>(countries);

        var options = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(60),
            AbsoluteExpiration = DateTime.UtcNow.AddHours(12)
        };

        string model = JsonConvert.SerializeObject(mapped);
        byte[] data = Encoding.UTF8.GetBytes(model);
        return distributedCache.SetAsync(cacheKey, data, options);
    }


    private Task SetMemoryCache()
    {
        memoryCache.Remove(cacheKey);

        var countries = unitOfWork.CountryRepository.GetAllAsync().Result;
        if (countries == null)
            return Task.CompletedTask;

        var mapped = mapper.Map<List<CountryResponse>>(countries);

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(60),
            AbsoluteExpiration = DateTime.UtcNow.AddHours(12)
        };

        memoryCache.Set(cacheKey, mapped, options);

        return Task.CompletedTask;
    }
}
