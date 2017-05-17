using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sof.JCDecaux.Vls
{
	public class VlsClient
	{
		/// <summary>
		/// Gets the API key.
		/// </summary>
		/// <value>The API key.</value>
		public string ApiKey { get; }

		private readonly HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sof.JCDecaux.Vls.VlsClient"/> class.
		/// </summary>
		/// <param name="apiKey">API key (available for free at https://developer.jcdecaux.com).</param>
		/// <param name="httpClient">Optionally provide an instance of HttpClient (primarily used for testing).</param>
		public VlsClient(string apiKey, HttpClient httpClient = null)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException("apiKey required (available for free at https://developer.jcdecaux.com)");

			ApiKey = apiKey;

			client = httpClient ?? new HttpClient();
		}

		/// <summary>
		/// Retrieve information about all contracts.
		/// </summary>
		/// <returns>A collection of contracts.</returns>
		public async Task<List<VlsContract>> GetContracts()
		{
			string uri = $"https://api.jcdecaux.com/vls/v1/contracts?apiKey={ApiKey}";

			return await GetResponse<List<VlsContract>>(uri);
		}

		/// <summary>
		/// Retrieve static and realtime information about a specific station.
		/// </summary>
		/// <returns>The station.</returns>
		/// <param name="contract">The contract name.</param>
		/// <param name="number">The station number.</param>
		public async Task<VlsStation> GetStation(string contract, int number)
		{
			string uri = $"https://api.jcdecaux.com/vls/v1/stations/{number}?apiKey={ApiKey}&contract={contract}";

			return await GetResponse<VlsStation>(uri);
		}

		/// <summary>
		/// Retrieve static and realtime information about all stations, optionally filtering by contract.
		/// </summary>
		/// <returns>A collection of stations.</returns>
		/// <param name="contract">Optionally filter by contract name.</param>
		public async Task<List<VlsStation>> GetStations(string contract = null)
		{
			string uri = $"https://api.jcdecaux.com/vls/v1/stations?apiKey={ApiKey}";

			if (!String.IsNullOrEmpty(contract))
				uri += $"&contract={contract}";

			return await GetResponse<List<VlsStation>>(uri);
		}

		private async Task<T> GetResponse<T>(string uri)
		{
			HttpResponseMessage response = null;

			response = await client.GetAsync(uri);

			if (response.StatusCode == HttpStatusCode.Forbidden)
				throw new InvalidOperationException("Incorrect or no API key set. Request URI: " + uri);

			if (response.StatusCode == HttpStatusCode.NotFound)
				throw new InvalidOperationException("Resource not found, possible API changes. Request URI: " + uri);

			if (response.StatusCode == HttpStatusCode.BadRequest)
				throw new ArgumentException("Specified contract does not exist. Request URI: " + uri);

			string res = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(res);
		}
	}
}
