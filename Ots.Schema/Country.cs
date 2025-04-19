using Ots.Base;

namespace Ots.Schema;

public class CountryRequest : BaseRequest
{
    public string Name { get; set; }
    public string IsoCode { get; set; }
    public string PhoneCode { get; set; }
    public string Flag { get; set; }
}

public class CountryResponse : BaseResponse
{
    public string Name { get; set; }
    public string IsoCode { get; set; }
    public string PhoneCode { get; set; }
    public string Flag { get; set; }
}
