using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoInstances.Default.Tests.Domain.Constants;
using Splinter.NanoInstances.Default.Tests.Domain.Language;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge
{
    #region Base Classes

    public abstract class LanguagePhraseAgents : NanoAgent, ILanguagePhraseAgent
    {
        private readonly string _phrase;

        protected LanguagePhraseAgents(string phrase)
        {
            _phrase = phrase;
        }

        public Task<string> Speak()
        {
            return Task.FromResult(_phrase);
        }
    }

    public abstract class SayHelloPhraseAgent : LanguagePhraseAgents
    {
        public static readonly SplinterId NanoTypeId = LanguageSplinterIds.SayHelloNanoTypeId;

        protected SayHelloPhraseAgent(LanguagePack pack) : base(pack.Hello)
        { }
        
        public override SplinterId TypeId => NanoTypeId;
    }

    public abstract class SayTestPhraseAgent : LanguagePhraseAgents
    {
        public static readonly SplinterId NanoTypeId = LanguageSplinterIds.SayTestNanoTypeId;

        protected SayTestPhraseAgent(LanguagePack pack) : base(pack.Test)
        { }

        public override SplinterId TypeId => NanoTypeId;
    }

    public abstract class SayGoodbyePhraseAgent : LanguagePhraseAgents
    {
        public static readonly SplinterId NanoTypeId = LanguageSplinterIds.SayGoodbyeNanoTypeId;

        protected SayGoodbyePhraseAgent(LanguagePack pack) : base(pack.Goodbye)
        { }

        public override SplinterId TypeId => NanoTypeId;
    }

    #endregion

    #region Afrikaans

    public class SayAfrikaansHelloPhraseAgent : SayHelloPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Afrikaans Hello Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{4EB34582-72D6-4B18-B590-9FD7134B038D}")
        };

        public SayAfrikaansHelloPhraseAgent() : base(LanguageConstants.Afrikaans)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayAfrikaansTestPhraseAgent : SayTestPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Afrikaans Test Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{358E8709-FA64-4E61-886C-096F1F016B79}")
        };

        public SayAfrikaansTestPhraseAgent() : base(LanguageConstants.Afrikaans)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayAfrikaansGoodbyePhraseAgent : SayGoodbyePhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Afrikaans Goodbye Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{D49AB8AC-E6BA-4FB4-9D56-00F1277C6337}")
        };

        public SayAfrikaansGoodbyePhraseAgent() : base(LanguageConstants.Afrikaans)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    #endregion

    #region English

    public class SayEnglishHelloPhraseAgent : SayHelloPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "English Hello Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{9C55DA4F-2525-4249-B83F-DF88E96F9D1D}")
        };

        public SayEnglishHelloPhraseAgent() : base(LanguageConstants.English)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayEnglishTestPhraseAgent : SayTestPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "English Test Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{C9C4666A-71AD-4C5A-ADC4-2D1002F85382}")
        };

        public SayEnglishTestPhraseAgent() : base(LanguageConstants.English)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayEnglishGoodbyePhraseAgent : SayGoodbyePhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "English Goodbye Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{9177C904-4684-4A9E-A2BA-998AABA27916}")
        };

        public SayEnglishGoodbyePhraseAgent() : base(LanguageConstants.English)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    #endregion

    #region French

    public class SayFrenchHelloPhraseAgent : SayHelloPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "French Hello Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{397532E2-26AF-4833-85B5-CB3217BE454C}")
        };

        public SayFrenchHelloPhraseAgent() : base(LanguageConstants.French)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayFrenchTestPhraseAgent : SayTestPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "French Test Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{47337B12-B20F-4924-82F7-C38D10212296}")
        };

        public SayFrenchTestPhraseAgent() : base(LanguageConstants.French)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayFrenchGoodbyePhraseAgent : SayGoodbyePhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "French Goodbye Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{73BC1760-4F3B-4EA4-9863-46E28026A9B3}")
        };

        public SayFrenchGoodbyePhraseAgent() : base(LanguageConstants.French)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    #endregion

    #region German

    public class SayGermanHelloPhraseAgent : SayHelloPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "German Hello Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{7442DA14-B9D0-4755-9C5F-0CDDFF117B57}")
        };

        public SayGermanHelloPhraseAgent() : base(LanguageConstants.German)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayGermanTestPhraseAgent : SayTestPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "German Test Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{AAB6234E-F2D0-4CDA-A2EC-3E005C058AD8}")
        };

        public SayGermanTestPhraseAgent() : base(LanguageConstants.German)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SayGermanGoodbyePhraseAgent : SayGoodbyePhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "German Goodbye Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{7E750AED-AA05-4BD7-A492-4B4F622A0773}")
        };

        public SayGermanGoodbyePhraseAgent() : base(LanguageConstants.German)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    #endregion

    #region Spanish

    public class SaySpanishHelloPhraseAgent : SayHelloPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Spanish Hello Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{CBBA39F5-E525-4BAE-91BE-82DD42D2ED24}")
        };

        public SaySpanishHelloPhraseAgent() : base(LanguageConstants.Spanish)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SaySpanishTestPhraseAgent : SayTestPhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Spanish Test Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{89819CA7-E689-445A-9B94-3D792E24C578}")
        };

        public SaySpanishTestPhraseAgent() : base(LanguageConstants.Spanish)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class SaySpanishGoodbyePhraseAgent : SayGoodbyePhraseAgent
    {
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Spanish Goodbye Phrase Agent",
            Version = "1.0.0",
            Guid = new Guid("{F5B80884-4F76-4AF3-806F-F5F818579737}")
        };

        public SaySpanishGoodbyePhraseAgent() : base(LanguageConstants.Spanish)
        { }

        public override SplinterId InstanceId => NanoInstanceId;
    }

    #endregion
}
