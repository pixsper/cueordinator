using System;
using System.Text.Json.Serialization;
using Pixsper.Cueordinator.Models.Disguise;
using Pixsper.Cueordinator.Models.Eos;

namespace Pixsper.Cueordinator.Models;

[JsonDerivedType(typeof(DisguiseConnectionConfiguration), typeDiscriminator: "disguise")]
[JsonDerivedType(typeof(EosConnectionConfiguration), typeDiscriminator: "eos")]
public interface IConnectionConfiguration
{
    [JsonIgnore]
    ConnectionKind Kind { get; }

    Guid Id { get; }

    string Label { get; }
}