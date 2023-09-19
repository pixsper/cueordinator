using System;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Pixsper.Cueordinator.JsonConverters;

internal class DisguiseDateTimeJsonConverter : JsonConverter<DateTime>
{
    public const string DisguiseDateFormat = "d MMM yyyy HH:mm:ss.fff";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString() ?? string.Empty, DisguiseDateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DisguiseDateFormat, CultureInfo.InvariantCulture).ToLowerInvariant());
    }
}