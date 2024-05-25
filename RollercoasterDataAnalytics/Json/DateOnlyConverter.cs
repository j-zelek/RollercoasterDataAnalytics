using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RollercoasterDataAnalytics.Json
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private const string FORMAT = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.ParseExact(reader.GetString() ?? throw new JsonException($"Could not read string for {typeToConvert.Name}"), FORMAT, CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(FORMAT));
    }
}
