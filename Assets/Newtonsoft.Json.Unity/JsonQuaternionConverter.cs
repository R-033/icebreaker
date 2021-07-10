using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Unity
{
    public class JsonQuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
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

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanWrite => true;

        public override bool CanRead => false;
    }
}
