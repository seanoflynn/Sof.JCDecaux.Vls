using System;
using System.Net;
using System.Net.Http;
using Xunit;
using RichardSzalay.MockHttp;

namespace Sof.JCDecaux.Vls.Tests
{
	public class VlsClientTests
	{
		[Fact]
		public void Contructor_ValidApiKey_ApiKeySetCorrectly()
		{			
			string key = "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764";

			var client = new VlsClient(key);

			Assert.Equal(key, client.ApiKey);
		}

		[Fact]
		public void Constructor_NullApiKey_ThrowArgumentException()
		{
			string key = null;

			Assert.Throws<ArgumentException>(() => new VlsClient(key));
		}

		[Fact]
		public void Constructor_BlankApiKey_ThrowArgumentException()
		{
			string key = String.Empty;

			Assert.Throws<ArgumentException>(() => new VlsClient(key));
		}

		[Fact]
		public void GetContracts_ReturnsContracts()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/contracts")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
					.Respond("application/json", "[{\"name\":\"Rouen\",\"cities\":[\"Rouen\"],\"commercial_name\":\"cy'clic\",\"country_code\":\"FR\"},{\"name\":\"Dublin\",\"cities\":[\"Dublin\"],\"commercial_name\":\"dublinbikes\",\"country_code\":\"IE\"},{\"name\":\"Lyon\",\"cities\":[\"Caluire-et-Cuire\",\"Lyon\",\"Vaulx-En-Velin\",\"Villeurbanne\"],\"commercial_name\":\"Vélo'V\",\"country_code\":\"FR\"},{\"name\":\"Besancon\",\"cities\":[\"Besançon\"],\"commercial_name\":\"VéloCité\",\"country_code\":\"FR\"}]");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			var contracts = client.GetContracts().Result;

			Assert.NotNull(contracts);
			Assert.Equal(contracts.Count, 4);

			Assert.Equal("Rouen", contracts[0].Name);
			Assert.Equal("cy'clic", contracts[0].CommercialName);
			Assert.Equal("FR", contracts[0].CountryCode);
			Assert.Equal(new[] { "Rouen" }, contracts[0].Cities);

			Assert.Equal("Dublin", contracts[1].Name);
			Assert.Equal("dublinbikes", contracts[1].CommercialName);
			Assert.Equal("IE", contracts[1].CountryCode);
			Assert.Equal(new[] { "Dublin" }, contracts[1].Cities);

			Assert.Equal("Lyon", contracts[2].Name);
			Assert.Equal("Vélo'V", contracts[2].CommercialName);
			Assert.Equal("FR", contracts[2].CountryCode);
			Assert.Equal(new[] { "Caluire-et-Cuire", "Lyon", "Vaulx-En-Velin", "Villeurbanne" }, contracts[2].Cities);

			Assert.Equal("Besancon", contracts[3].Name);
			Assert.Equal("VéloCité", contracts[3].CommercialName);
			Assert.Equal("FR", contracts[3].CountryCode);
			Assert.Equal(new[] { "Besançon" }, contracts[3].Cities);
		}

		[Fact]
		public void GetStation_ReturnStation()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/stations/12")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
					.WithQueryString("contract", "Dublin")
					.Respond("application/json", "{\"number\":12,\"name\":\"ECCLES STREET\",\"address\":\"Eccles Street\",\"position\":{\"lat\":53.359246,\"lng\":-6.269779},\"banking\":false,\"bonus\":false,\"status\":\"OPEN\",\"contract_name\":\"Dublin\",\"bike_stands\":20,\"available_bike_stands\":6,\"available_bikes\":14,\"last_update\":1494964821000}");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			var station = client.GetStation("Dublin", 12).Result;

			Assert.NotNull(station);
			Assert.Equal(12, station.Number);
			Assert.Equal("ECCLES STREET", station.Name);
			Assert.Equal("Eccles Street", station.Address);
			Assert.NotNull(station.Position);
			Assert.Equal(53.359246m, station.Position.Latitude);
			Assert.Equal(-6.269779m, station.Position.Longtitude);
			Assert.Equal(false, station.Banking);
			Assert.Equal(false, station.Bonus);
			Assert.Equal(VlsStationState.Open, station.Status);
			Assert.Equal("Dublin", station.ContractName);
			Assert.Equal(20, station.BikeStands);
			Assert.Equal(6, station.AvailableBikeStands);
			Assert.Equal(14, station.AvailableBikes);
			Assert.Equal(new DateTime(2017, 5, 16, 20, 0, 21, DateTimeKind.Utc), station.LastUpdate);
		}

		[Fact]
		public void GetStations_ReturnStations()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/stations")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
					.Respond("application/json", "[{\"number\":280,\"name\":\"280 - HEYSEL / HEISEL\",\"address\":\"HEYSEL / HEISEL - AVENUE DE L'IMPERATRICE CHARLOTTE / KEIZERIN CHARLOTTELAAN\",\"position\":{\"lat\":50.897522,\"lng\":4.334831},\"banking\":true,\"bonus\":false,\"status\":\"OPEN\",\"contract_name\":\"Bruxelles-Capitale\",\"bike_stands\":25,\"available_bike_stands\":18,\"available_bikes\":6,\"last_update\":1494964745000},{\"number\":31705,\"name\":\"31705 - CHAMPEAUX (BAGNOLET)\",\"address\":\"RUE DES CHAMPEAUX (PRES DE LA GARE ROUTIERE) - 93170 BAGNOLET\",\"position\":{\"lat\":48.8645278209514,\"lng\":2.416170724425901},\"banking\":true,\"bonus\":true,\"status\":\"OPEN\",\"contract_name\":\"Paris\",\"bike_stands\":50,\"available_bike_stands\":50,\"available_bikes\":0,\"last_update\":1494964728000},{\"number\":254,\"name\":\"254 - BASILIQUE / BASILIEK\",\"address\":\"BASILIQUE / BASILIEK - AV JACQUES SERMON / JACUES SERMONLAAN\",\"position\":{\"lat\":50.867393,\"lng\":4.320344},\"banking\":true,\"bonus\":false,\"status\":\"OPEN\",\"contract_name\":\"Bruxelles-Capitale\",\"bike_stands\":22,\"available_bike_stands\":17,\"available_bikes\":5,\"last_update\":1494964818000}]");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			var stations = client.GetStations().Result;

			Assert.NotNull(stations);
			Assert.Equal(stations.Count, 3);

			Assert.Equal(280, stations[0].Number);
			Assert.Equal("280 - HEYSEL / HEISEL", stations[0].Name);
			Assert.Equal("HEYSEL / HEISEL - AVENUE DE L'IMPERATRICE CHARLOTTE / KEIZERIN CHARLOTTELAAN", stations[0].Address);
			Assert.NotNull(stations[0].Position);
			Assert.Equal(50.897522m, stations[0].Position.Latitude);
			Assert.Equal(4.334831m, stations[0].Position.Longtitude);
			Assert.Equal(true, stations[0].Banking);
			Assert.Equal(false, stations[0].Bonus);
			Assert.Equal(VlsStationState.Open, stations[0].Status);
			Assert.Equal("Bruxelles-Capitale", stations[0].ContractName);
			Assert.Equal(25, stations[0].BikeStands);
			Assert.Equal(18, stations[0].AvailableBikeStands);
			Assert.Equal(6, stations[0].AvailableBikes);
			Assert.Equal(new DateTime(2017, 5, 16, 19, 59, 5, DateTimeKind.Utc), stations[0].LastUpdate);

			Assert.Equal(31705, stations[1].Number);
			Assert.Equal("31705 - CHAMPEAUX (BAGNOLET)", stations[1].Name);
			Assert.Equal("RUE DES CHAMPEAUX (PRES DE LA GARE ROUTIERE) - 93170 BAGNOLET", stations[1].Address);
			Assert.NotNull(stations[1].Position);
			Assert.Equal(48.8645278209514m, stations[1].Position.Latitude);
			Assert.Equal(2.416170724425901m, stations[1].Position.Longtitude);
			Assert.Equal(true, stations[1].Banking);
			Assert.Equal(true, stations[1].Bonus);
			Assert.Equal(VlsStationState.Open, stations[1].Status);
			Assert.Equal("Paris", stations[1].ContractName);
			Assert.Equal(50, stations[1].BikeStands);
			Assert.Equal(50, stations[1].AvailableBikeStands);
			Assert.Equal(0, stations[1].AvailableBikes);
			Assert.Equal(new DateTime(2017, 5, 16, 19, 58, 48, DateTimeKind.Utc), stations[1].LastUpdate);

			Assert.Equal(254, stations[2].Number);
			Assert.Equal("254 - BASILIQUE / BASILIEK", stations[2].Name);
			Assert.Equal("BASILIQUE / BASILIEK - AV JACQUES SERMON / JACUES SERMONLAAN", stations[2].Address);
			Assert.NotNull(stations[2].Position);
			Assert.Equal(50.867393m, stations[2].Position.Latitude);
			Assert.Equal(4.320344m, stations[2].Position.Longtitude);
			Assert.Equal(true, stations[2].Banking);
			Assert.Equal(false, stations[2].Bonus);
			Assert.Equal(VlsStationState.Open, stations[2].Status);
			Assert.Equal("Bruxelles-Capitale", stations[2].ContractName);
			Assert.Equal(22, stations[2].BikeStands);
			Assert.Equal(17, stations[2].AvailableBikeStands);
			Assert.Equal(5, stations[2].AvailableBikes);
			Assert.Equal(new DateTime(2017, 5, 16, 20, 0, 18, DateTimeKind.Utc), stations[2].LastUpdate);
		}

		[Fact]
		public void GetStation_ContractSpecified_ReturnStation()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/stations")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
					.WithQueryString("contract", "Dublin")
					.Respond("application/json", "[{\"number\":42,\"name\":\"SMITHFIELD NORTH\",\"address\":\"Smithfield North\",\"position\":{\"lat\":53.349562,\"lng\":-6.278198},\"banking\":true,\"bonus\":false,\"status\":\"OPEN\",\"contract_name\":\"Dublin\",\"bike_stands\":30,\"available_bike_stands\":0,\"available_bikes\":29,\"last_update\":1494964926000},{\"number\":88,\"name\":\"BLACKHALL PLACE\",\"address\":\"Blackhall Place\",\"position\":{\"lat\":53.3488,\"lng\":-6.281637},\"banking\":false,\"bonus\":false,\"status\":\"OPEN\",\"contract_name\":\"Dublin\",\"bike_stands\":30,\"available_bike_stands\":0,\"available_bikes\":30,\"last_update\":1494964759000}]");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			var stations = client.GetStations("Dublin").Result;

			Assert.NotNull(stations);
			Assert.Equal(stations.Count, 2);

			Assert.Equal(42, stations[0].Number);
			Assert.Equal("SMITHFIELD NORTH", stations[0].Name);
			Assert.Equal("Smithfield North", stations[0].Address);
			Assert.NotNull(stations[0].Position);
			Assert.Equal(53.349562m, stations[0].Position.Latitude);
			Assert.Equal(-6.278198m, stations[0].Position.Longtitude);
			Assert.Equal(true, stations[0].Banking);
			Assert.Equal(false, stations[0].Bonus);
			Assert.Equal(VlsStationState.Open, stations[0].Status);
			Assert.Equal("Dublin", stations[0].ContractName);
			Assert.Equal(30, stations[0].BikeStands);
			Assert.Equal(0, stations[0].AvailableBikeStands);
			Assert.Equal(29, stations[0].AvailableBikes);
			Assert.Equal(new DateTime(2017, 5, 16, 20, 2, 6, DateTimeKind.Utc), stations[0].LastUpdate);

			Assert.Equal(88, stations[1].Number);
			Assert.Equal("BLACKHALL PLACE", stations[1].Name);
			Assert.Equal("Blackhall Place", stations[1].Address);
			Assert.NotNull(stations[1].Position);
			Assert.Equal(53.3488m, stations[1].Position.Latitude);
			Assert.Equal(-6.281637m, stations[1].Position.Longtitude);
			Assert.Equal(false, stations[1].Banking);
			Assert.Equal(false, stations[1].Bonus);
			Assert.Equal(VlsStationState.Open, stations[1].Status);
			Assert.Equal("Dublin", stations[1].ContractName);
			Assert.Equal(30, stations[1].BikeStands);
			Assert.Equal(0, stations[1].AvailableBikeStands);
			Assert.Equal(30, stations[1].AvailableBikes);
			Assert.Equal(new DateTime(2017, 5, 16, 19, 59, 19, DateTimeKind.Utc), stations[1].LastUpdate);
		}

		[Fact]
		public void GetStations_InvalidContract_ThrowsArgumentException()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/stations")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
					.WithQueryString("contract", "Galway")
			        .Respond(HttpStatusCode.BadRequest, "application/json", "{ \"error\" : \"Specified contract does not exist\" }");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(async () => await client.GetStations("Galway")).Result;
			Assert.Contains("Specified contract",ex.Message);
		}

		[Fact]
		public void GetContracts_InvalidApiKey_ThrowsInvalidOperationException()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/contracts")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
			        .Respond(HttpStatusCode.Forbidden, "application/json", "{ \"error\" : \"Unauthorized\" }");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			InvalidOperationException ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetContracts()).Result;
			Assert.Contains("API key", ex.Message);
		}

		[Fact]
		public void GetContracts_EndpointChanged_ThrowsInvalidOperationException()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/contracts")
					.WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
			        .Respond(HttpStatusCode.NotFound, "application/json", "{ \"error\" : \"Resource Not Found\" }");

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			InvalidOperationException ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetContracts()).Result;
			Assert.Contains("Resource not found", ex.Message);
		}

		[Fact]
		public void GetContracts_NoInternetConnection_ThrowsAggregateException()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.When("https://api.jcdecaux.com/vls/v1/contracts")
			        .WithQueryString("apiKey", "1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764")
			        .Throw(new AggregateException(new [] { new HttpRequestException("An error occurred while sending the request.") }));

			var httpClient = mockHttp.ToHttpClient();

			var client = new VlsClient("1f4edfe1d2fd17c2ffb1f4c2e403797c5a019764", httpClient);

			AggregateException ex = Assert.ThrowsAsync<AggregateException>(async () => await client.GetContracts()).Result;
			Assert.IsType(typeof(HttpRequestException), ex.InnerException);
		}
	}
}
