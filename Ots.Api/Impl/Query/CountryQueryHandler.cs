using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Ots.Api.Impl.Cqrs;
using Ots.Base;
using Ots.Schema;

namespace Ots.Api.Impl.Query;

public class CountryQueryHandler :
IRequestHandler<GetAllCountryQuery, ApiResponse<List<CountryResponse>>>,
IRequestHandler<GetCountryByIdQuery, ApiResponse<CountryResponse>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDistributedCache distributedCache;
    private readonly IMemoryCache memoryCache;
    private readonly string cacheKey = "CountryList-78";


    public CountryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCache, IMemoryCache memoryCache)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.distributedCache = distributedCache;
        this.memoryCache = memoryCache;
    }

    public async Task<ApiResponse<List<CountryResponse>>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
    {
        // check if cache exists
        if (request.CacheType == "Memory")
        {
            var cashResult = memoryCache.TryGetValue(cacheKey, out List<CountryResponse> cashValue);
            if (cashResult && cashValue != null)
            {
                return new ApiResponse<List<CountryResponse>>(cashValue);
            }
            return await SetMemoryCache();
        }
        else if (request.CacheType == "Redis")
        {
            var cashResult = await distributedCache.GetAsync(cacheKey);
            if (cashResult != null)
            {
                string json = Encoding.UTF8.GetString(cashResult);
                var cachedResponse = JsonConvert.DeserializeObject<List<CountryResponse>>(json);
                return new ApiResponse<List<CountryResponse>>(cachedResponse);
            }
            return await SetRedisCache();
        }
        else
        {
            return new ApiResponse<List<CountryResponse>>("Invalid cache type");
        }
    }

    public async Task<ApiResponse<CountryResponse>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var Country = await unitOfWork.CountryRepository.GetByIdAsync(request.Id);
        var mapped = mapper.Map<CountryResponse>(Country);
        return new ApiResponse<CountryResponse>(mapped);
    }

    private async Task<ApiResponse<List<CountryResponse>>> SetRedisCache()
    {
        var countryList = await unitOfWork.CountryRepository.GetAllAsync();
        var mapped = mapper.Map<List<CountryResponse>>(countryList);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(60),
            AbsoluteExpiration = DateTime.UtcNow.AddHours(12)
        };

        var cacheValue = JsonConvert.SerializeObject(mapped);
        byte[] data = Encoding.UTF8.GetBytes(cacheValue);
        await distributedCache.SetAsync(cacheKey, data, cacheOptions);

        return new ApiResponse<List<CountryResponse>>(mapped);
    }

    private async Task<ApiResponse<List<CountryResponse>>> SetMemoryCache()
    {
        var countryList = await unitOfWork.CountryRepository.GetAllAsync();
        var mapped = mapper.Map<List<CountryResponse>>(countryList);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(60),
            AbsoluteExpiration = DateTime.UtcNow.AddHours(12)
        };
        
        memoryCache.Set(cacheKey, mapped, cacheOptions);
        return new ApiResponse<List<CountryResponse>>(mapped);
    }
}
