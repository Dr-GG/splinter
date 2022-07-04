using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.Applications.Test.Domain.Constants;

public static class TeraMessageSplinterIds
{
    public static readonly SplinterId TeraTypeId = new()
    {
        Name = "Tera Agent",
        Version = "1.0.0",
        Guid = new Guid("{E9B4DFEF-DE78-4600-ACB4-6C034E7E0BA7}")
    };
    public static readonly SplinterId KnowledgeNanoTypeId = new()
    {
        Name = "Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{30D71214-B488-41D6-A4FF-010B15364452}")
    };
}