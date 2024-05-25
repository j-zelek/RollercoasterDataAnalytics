using System.Text.Json;
using System.Text.Json.Serialization;

namespace RollercoasterDataAnalytics.Json
{
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private const string FORMAT = "HH:mm";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => TimeOnly.ParseExact(reader.GetString() ?? throw new JsonException($"Could not read value for {typeToConvert.Name}"), FORMAT);

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(FORMAT));
    }
}
