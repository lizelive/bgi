using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

public class MaybeArrayConverter<T> : CustomCreationConverter<MaybeArray<T>>
{
    public override MaybeArray<T> Create(Type objectType)
    {
        return new MaybeArray<T>();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            var lol = reader.ReadAsString();
            //base.ReadJson(reader, typeof(T[]) )
        }
        return base.ReadJson(reader, objectType, existingValue, serializer);
    }
}
