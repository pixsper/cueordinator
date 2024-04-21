using System;
using System.Net;
using System.Text.Json.Serialization;
using Pixsper.Cueordinator.JsonConverters;

namespace Pixsper.Cueordinator.Models.Eos;

public record EosConnectionConfiguration : IConnectionConfiguration
{
    [JsonIgnore]
    public ConnectionKind Kind => ConnectionKind.Eos;

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Label { get; init; } = "Eos Console";

    [JsonConverter(typeof(IpAddressJsonConverter))]
    public IPAddress Address { get; init; } = IPAddress.Loopback;
}