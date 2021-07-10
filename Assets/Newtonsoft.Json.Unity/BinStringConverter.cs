using Hypercooled.Utils;
using System;

namespace Newtonsoft.Json.Unity
{
	public class BinStringConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(uint) || objectType == typeof(string);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return 0;
			}
			if (reader.TokenType == JsonToken.String)
			{
				var name = (string)reader.Value;
				return name.BinHash();
			}
			if (reader.TokenType == JsonToken.Integer)
			{
				return (uint)reader.Value;
			}

			throw new JsonSerializationException($"Unexpected token or value when parsing version. Token: {reader.ValueType}, Value: {reader.Value}");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is null)
			{
				writer.WriteNull();
				return;
			}

			var key = (uint)value;
			var name = key.BinString();
			writer.WriteValue(name);
		}
	}
}
