# Sof.JCDecaux.Vls

Access JCDecaux VLS bike sharing realtime data API.

## VlsClient

You can use the `VlsClient` class to retrieve contract and station information.

You need an API key which you can get for free from https://developer.jcdecaux.com/.

A contract refers to each city or group of cities that JCDecaux has a contract for bike sharing with.

```csharp
public class VlsContract
{
    public string Name { get; set; }
    public string CommercialName { get; set; }
    public string CountryCode { get; set; }

    public List<string> Cities { get; set; }
}
```

A station is an individual bike station.

```csharp
public class VlsStation
{
    public string ContractName { get; set; }
    public int Number { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public bool Banking { get; set; }
    public bool Bonus { get; set; }
    public VlsStationState Status { get; set; } // StationState.Open or StationState.Closed
    public VlsStationPosition Position { get; set; } // contains Latitude and Longtitude

    public int BikeStands { get; set; }
    public int AvailableBikeStands { get; set; }
    public int AvailableBikes { get; set; }

    public DateTime LastUpdate { get; set; }
}
```

Below are some examples.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sof.JCDecaux.Vls.Examples
{
    public class Program
    {
        public static void Main()
        {
            Examples().Wait();
        }

        public static async Task Examples()
        {
            var client = new VlsClient("{your api key here}");

            // retrieve a list of contracts
            List<VlsContract> contracts = await client.GetContracts();

            // retrieve a list of all stations (from all contracts)
            List<VlsStation> stations = await client.GetStations();

            // retrieve a list of all stations for a specific contract
            List<VlsStation> stationsInDublin = await client.GetStations("Dublin");

            // retrieve a single station
            VlsStation station = await client.GetStation("Dublin", 18);
        }
    }
}
```

