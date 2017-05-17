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
			var client = new VlsClient("{your api key}");

			List<VlsContract> contracts = await client.GetContracts();

			List<VlsStation> stations = await client.GetStations();

			List<VlsStation> stationsInDublin = await client.GetStations("Dublin");

			VlsStation station = await client.GetStation("Dublin", 18);
		}
	}
}
