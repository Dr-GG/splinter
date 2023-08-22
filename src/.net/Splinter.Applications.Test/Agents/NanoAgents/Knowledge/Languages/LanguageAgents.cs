using System;
using System.Threading.Tasks;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge.Languages;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge.Languages.Phrases;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.Applications.Test.Agents.NanoAgents.Knowledge.Languages;

#region Base Class

public abstract class LanguageAgent : NanoAgent, ILanguageAgent
{
    private INanoReference? _helloPhraseAgent;
    private INanoReference? _testPhraseAgent;
    private INanoReference? _goodbyePhraseAgent;

    public static readonly SplinterId NanoTypeId = LanguageSplinterIds.LanguageNanoTypeId;

    public override SplinterId TypeId => NanoTypeId;

    public async Task<string> SayHello()
    {
        return await SayPhrase(_helloPhraseAgent);
    }

    public async Task<string> SayTest()
    {
        return await SayPhrase(_testPhraseAgent);
    }

    public async Task<string> SayGoodbye()
    {
        return await SayPhrase(_goodbyePhraseAgent);
    }

    protected override async Task InitialiseNanoReferences()
    {
        await base.InitialiseNanoReferences();

        _helloPhraseAgent = await CollapseNanoReference(LanguageSplinterIds.SayHelloNanoTypeId);
        _testPhraseAgent = await CollapseNanoReference(LanguageSplinterIds.SayTestNanoTypeId);
        _goodbyePhraseAgent = await CollapseNanoReference(LanguageSplinterIds.SayGoodbyeNanoTypeId);
    }

    protected override async Task DisposeNanoReferences()
    {
        await base.DisposeNanoReferences();

        await DisposeNanoReference(_helloPhraseAgent);
        await DisposeNanoReference(_testPhraseAgent);
        await DisposeNanoReference(_goodbyePhraseAgent);
    }

    private static async Task<string> SayPhrase(INanoReference? reference)
    {
        if (reference.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Language phrase agent not set.");
        }

        return await reference.Typed<ILanguagePhraseAgent>().Speak();
    }
}

#endregion

#region Afrikaans

public class AfrikaansLanguageAgent : LanguageAgent
{

    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Afrikaans Language Agent",
        Version = "1.0.0",
        Guid = new Guid("{13167C25-7923-4729-B3D4-C5B3F3833EB9}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region English

public class EnglishLanguageAgent : LanguageAgent
{

    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "English Language Agent",
        Version = "1.0.0",
        Guid = new Guid("{C169236E-82A6-4F27-B422-ACB883734C28}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region French

public class FrenchLanguageAgent : LanguageAgent
{

    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "French Language Agent",
        Version = "1.0.0",
        Guid = new Guid("{3D364088-8581-44F0-BEEB-06D2E97ED770}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region German

public class GermanLanguageAgent : LanguageAgent
{

    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "German Language Agent",
        Version = "1.0.0",
        Guid = new Guid("{D5176C69-51A8-4FE3-98FB-D46F1966A45B}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region Spanish

public class SpanishLanguageAgent : LanguageAgent
{

    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Spanish Language Agent",
        Version = "1.0.0",
        Guid = new Guid("{A772798B-39F9-4289-B4D8-A286D278467C}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion