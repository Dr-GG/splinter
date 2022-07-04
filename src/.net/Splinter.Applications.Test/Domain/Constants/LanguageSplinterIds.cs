using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.Applications.Test.Domain.Constants;

public static class LanguageSplinterIds
{
    public static readonly SplinterId TeraTypeId = new()
    {
        Name = "Tera Agent",
        Version = "1.0.0",
        Guid = new Guid("{379330BC-1E41-4F1A-ABBB-4B3C8495DA27}")
    };
    public static readonly SplinterId KnowledgeNanoTypeId = new()
    {
        Name = "Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{56102517-7330-416D-BF10-30AD61F698E8}")
    };
    public static readonly SplinterId LanguageNanoTypeId = new()
    {
        Name = "Language Agent",
        Version = "1.0.0",
        Guid = new Guid("{367EE9EA-5489-42A5-83A1-DF86E0E8FC10}")
    };
    public static readonly SplinterId SayHelloNanoTypeId = new()
    {
        Name = "Hello Agent",
        Version = "1.0.0",
        Guid = new Guid("{D4B5B08D-AE39-486C-B010-812D1AE07EC5}")
    };
    public static readonly SplinterId SayTestNanoTypeId = new()
    {
        Name = "Test Agent",
        Version = "1.0.0",
        Guid = new Guid("{498B852B-8479-4505-A19B-638CF2E33E2E}")
    };
    public static readonly SplinterId SayGoodbyeNanoTypeId = new()
    {
        Name = "Goodbye Agent",
        Version = "1.0.0",
        Guid = new Guid("{EB47C49E-272D-42B5-8A63-5ADFED0F75AF}")
    };
}