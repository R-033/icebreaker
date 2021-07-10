using System;

namespace Newtonsoft.Json.Unity
{
	public class ByteArrayConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(byte[]);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			var array = Linq.JArray.Load(reader);
			var result = new byte[array.Count];
			var type = typeof(byte);

			for (int i = 0; i < array.Count; ++i)
			{
				var value = serializer.Deserialize(array[i].CreateReader(), type);
				result[i] = (byte)value;
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is null)
			{
				writer.WriteNull();
				return;
			}

			var array = value as byte[];
			writer.WriteStartArray();

			for (int i = 0; i < array.Length; ++i)
			{
				writer.WriteValue(array[i]);
			}

			writer.WriteEndArray();
		}
	}
}
