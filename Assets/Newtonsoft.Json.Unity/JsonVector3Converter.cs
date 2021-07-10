using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Unity
{
    public class JsonVector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            var obj = new JObject
			{
				{ "x", value.x },
				{ "y", value.y },
				{ "z", value.z }
			};

            obj.WriteTo(writer);
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanWrite => true;

        public override bool CanRead => false;
    }
}
