using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Domain.Constants;

public static class SplinterIdConstants
{
    public const string NanoTypeId = "NanoTypeId";
    public const string NanoInstanceId = "NanoInstanceId";

    public static readonly SplinterId SuperpositionAgentNanoTypeId = new()
    {
        Name = "Superposition Agent Tera Type",
        Version = "1.0.0",
        Guid = new Guid("{17AD25A3-3197-4EB6-BBB1-9FF6A0046EA9}")
    };

    public static readonly SplinterId TeraPlatformAgentNanoTypeId = new()
    {
        Name = "Tera Platform Agent Tera Type",
        Version = "1.0.0",
        Guid = new Guid("{92680E68-1E77-4AE8-8E38-1A1A551C512C}")
    };

    public static readonly SplinterId TeraRegistryAgentNanoTypeId = new()
    {
        Name = "Tera Registry Agent Tera Type",
        Version = "1.0.0",
        Guid = new Guid("{C0EE8937-3B4B-4AA0-8BFC-511D56A13393}")
    };

    public static readonly SplinterId TeraMessageAgentNanoTypeId = new()
    {
        Name = "Tera Agent Message Service",
        Version = "1.0.0",
        Guid = new Guid("{9681AA07-7B5F-4E45-B6FD-F56669F46469}")
    };

    public static readonly SplinterId TeraDefaultKnowledgeNanoTypeId = new()
    {
        Name = "Tera Default Knowledge",
        Version = "1.0.0",
        Guid = new Guid("{27051C02-AA88-47C2-B083-E59B0F5BB51C}")
    };
}