using System;
using System.Net;
using System.Text.Json.Serialization;
using Pixsper.Cueordinator.JsonConverters;

namespace Pixsper.Cueordinator.Models.Disguise;

public record DisguiseConnectionConfiguration : IConnectionConfiguration
{
    [JsonIgnore]
    public ConnectionKind Kind => ConnectionKind.Disguise;
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Label { get; init; } = "Disguise Server";

    [JsonConverter(typeof(IpAddressJsonConverter))]
    public IPAddress Address { get; init; } = IPAddress.Loopback;

    public int Port { get; init; } = 80;
}