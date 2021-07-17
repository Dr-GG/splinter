using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.Domain.Constants
{
    public static class LanguageSplinterIds
    {
        public static readonly Guid AfrikaansTeraId = new("{E1FB016E-22DA-49A6-963E-CDD56ACF02F0}");
        public static readonly Guid EnglishTeraId = new("{326E7729-B2E8-42D1-9ACB-5D5E409EE644}");
        public static readonly Guid FrenchTeraId = new("{9F0F869E-97E6-4076-AE4D-84BE1E7781FE}");
        public static readonly Guid GermanTeraId = new("{59BAA776-D110-495A-A1DE-29AB070F2D27}");
        public static readonly Guid SpanishTeraId = new("{3E477FAA-BAA1-469D-8FCF-651F3690FEFC}");

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
}
