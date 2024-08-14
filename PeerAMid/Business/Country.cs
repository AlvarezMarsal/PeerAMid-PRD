namespace PeerAMid.Business;

#nullable enable

public record class Country
{
    public string Name { get; }
    public string Currency { get; }
    public int RegionId { get; }

    public Country(string name, string currency, int regionId)
    {
        Name = name;
        Currency = currency;
        RegionId = regionId;
    }
}
