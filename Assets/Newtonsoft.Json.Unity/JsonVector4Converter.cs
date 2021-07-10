using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Unity
{
    public class JsonVector4Converter : JsonConverter<Vector4>
    {
        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            var obj = new JObject
			{
				{ "x", value.x },
				{ "y", value.y },
				{ "z", value.z },
				{ "w", value.w }
			};

            obj.WriteTo(writer);
        }

        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanWrite => true;

        public override bool CanRead => false;
    }
}
