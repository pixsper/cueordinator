using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Pixsper.Cueordinator.JsonConverters;

internal class IpAddressJsonConverter : JsonConverter<IPAddress>
{
    public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringValue = reader.GetString();
        if (IPAddress.TryParse(stringValue, out var value))
            return value;
        else
            throw new JsonException($"Failed to parse IP address from string '{stringValue}'");
    }

    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}