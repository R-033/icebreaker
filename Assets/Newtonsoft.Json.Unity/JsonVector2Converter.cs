using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Unity
{
    public class JsonVector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            var obj = new JObject
			{
				{ "x", value.x },
				{ "y", value.y }
			};

            obj.WriteTo(writer);
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanWrite => true;

        public override bool CanRead => false;
    }
}
