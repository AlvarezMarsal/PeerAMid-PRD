namespace PeerAMid.Business;

#nullable enable

public class Region
{
    public int Id { get; }
    public string Name { get; }
    //public string Currency { get; }
    public int NumberOfCompanies { get; set; }

    public Region(int id, string name, /*string currency,*/ int numberOfCompanies = 0)
    {
        Id = id;
        Name = name;
        //Currency = currency;
        NumberOfCompanies = numberOfCompanies;
    }
}
