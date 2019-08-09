using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mc
{
    [JsonConverter(typeof(MulitpartConditionConverter))]
    public class MulitpartCondition : Dictionary<string, string>
    {
        public MulitpartCondition[] OR, AND;
    }

    public class MulitpartConditionConverter : JsonConverter<MulitpartCondition>
    {
        public override MulitpartCondition ReadJson(JsonReader reader, Type objectType, MulitpartCondition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!hasExistingValue)
                existingValue = new MulitpartCondition();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                if ("OR".Equals(reader.Value))
                {
                    reader.Read();
                    existingValue.OR = serializer.Deserialize<MulitpartCondition[]>(reader);
                }
                else if ("AND".Equals(reader.Value))
                {
                    reader.Read();
                    existingValue.AND = serializer.Deserialize<MulitpartCondition[]>(reader);
                }
                else
                {
                    var key = (string)reader.Value;
                    var value = reader.ReadAsString();
                    existingValue[key] = value;
                }
            }

            return existingValue;
        }
        public override void WriteJson(JsonWriter writer, MulitpartCondition value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value.OR != null)
            {
                writer.WritePropertyName("OR");
                serializer.Serialize(writer, value.OR);
            }
            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key);
                writer.WriteValue(kvp.Value);
            }

            writer.WriteEndObject();
        }
    }
}
