using System;
using System.IO;
using Xunit;
using Newtonsoft.Json;

namespace Sof.JCDecaux.Vls.Tests
{
	public class TimestampConverterTests
	{		
		[Fact]
		public void CanConvert_GivenDateTimeType_ReturnsTrue()
		{
			var tc = new TimestampConverter();

			Assert.Equal(true, tc.CanConvert(typeof(DateTime)));
		}

		[Fact]
		public void CanConvert_GivenObjectType_ReturnsFalse()
		{
			var tc = new TimestampConverter();

			Assert.Equal(false, tc.CanConvert(typeof(object)));
		}

		[Fact]
		public void WriteJson_ThowsNotImplemented()
		{
			var tc = new TimestampConverter();

			Assert.Throws<NotImplementedException>(() => tc.WriteJson(null, null, null));
		}

		[Fact]
		public void ReadJson_GivenValidTimestamp_ConvertsToDateTime()
		{
			JsonTextReader reader = GetJsonReaderFromSingleToken("1494964821000");

			var tc = new TimestampConverter();

			var datetime = tc.ReadJson(reader, null, null, null);

			Assert.NotNull(datetime);
			Assert.Equal(new DateTime(2017, 5, 16, 20, 0, 21, DateTimeKind.Utc), datetime);
		}

		[Fact]
		public void ReadJson_GivenUnixEpochTimestamp_ConvertsToUnixEpochDateTime()
		{
			JsonTextReader reader = GetJsonReaderFromSingleToken("0");

			var tc = new TimestampConverter();

			var datetime = tc.ReadJson(reader, null, null, null);

			Assert.NotNull(datetime);
			Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), datetime);
		}

		private static JsonTextReader GetJsonReaderFromSingleToken(string input)
		{
			var memoryStream = new MemoryStream();
			var streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(input);
			streamWriter.Flush();
			memoryStream.Position = 0;

			var streamReader = new StreamReader(memoryStream);
			var reader = new JsonTextReader(streamReader);

			// read next token
			reader.Read();

			return reader;
		}
	}
}
