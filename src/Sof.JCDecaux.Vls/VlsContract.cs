using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sof.JCDecaux.Vls
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	public class VlsContract
	{
		public string Name { get; set; }
		public string CommercialName { get; set; }
		public string CountryCode { get; set; }

		public List<string> Cities { get; set; }
	}
}
