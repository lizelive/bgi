using Mc;
using Newtonsoft.Json;
using System;

[JsonConverter(typeof(MaybeArrayConverter))]
public class MaybeMcModelDescriptorArray : MaybeArray<McModelDescriptor>
{
    public MaybeMcModelDescriptorArray(McModelDescriptor[] value = null) : base(value)
    {
    }
}

public class MaybeArray<T>
{
    public T[] value;

    public MaybeArray(T[] value = null)
    {
        this.value = value;
    }
}
public class MaybeArrayConverter : JsonConverter<MaybeMcModelDescriptorArray>
{


    public override MaybeMcModelDescriptorArray ReadJson(JsonReader reader, Type objectType, MaybeMcModelDescriptorArray existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            return new MaybeMcModelDescriptorArray(serializer.Deserialize<McModelDescriptor[]>(reader));

        }

        return new MaybeMcModelDescriptorArray(new McModelDescriptor[] { serializer.Deserialize<McModelDescriptor>(reader) });
    }

    public override void WriteJson(JsonWriter writer, MaybeMcModelDescriptorArray value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.value);
    }
}