using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sof.JCDecaux.Vls
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	public class VlsStation
	{
		public string ContractName { get; set; }
		public int Number { get; set; }

		public string Name { get; set; }
		public string Address { get; set; }
		public bool Banking { get; set; }
		public bool Bonus { get; set; }
		public VlsStationState Status { get; set; }
		public VlsStationPosition Position { get; set; }

		public int BikeStands { get; set; }
		public int AvailableBikeStands { get; set; }
		public int AvailableBikes { get; set; }

		[JsonConverter(typeof(TimestampConverter))]
		public DateTime LastUpdate { get; set; }
	}

	public class VlsStationPosition
	{
		[JsonProperty("lat")]
		public decimal Latitude { get; set; }
		[JsonProperty("lng")]
		public decimal Longtitude { get; set; }
	}

	public enum VlsStationState
	{
		Open,
		Closed,
	}
}
