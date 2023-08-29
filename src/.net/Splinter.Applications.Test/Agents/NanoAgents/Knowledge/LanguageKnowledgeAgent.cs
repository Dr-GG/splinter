using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge.Languages;
using Splinter.NanoInstances.Default.Agents.NanoAgents.Knowledge;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.Applications.Test.Agents.NanoAgents.Knowledge;

#region Base Class

public abstract class LanguageKnowledgeAgent : TeraKnowledgeAgent, ILanguageKnowledgeAgent
{
    private readonly ICollection<string> _saidPhrases = new List<string>();

    private INanoReference? _languageReference;

    public new static readonly SplinterId NanoTypeId = LanguageSplinterIds.KnowledgeNanoTypeId;

    public override SplinterId TypeId => NanoTypeId;
    public IEnumerable<string> SaidPhrases => _saidPhrases.ToList();

    public override async Task Execute(TeraAgentExecutionParameters parameters)
    {
        await base.Execute(parameters);

        if (_languageReference.IsNullOrEmpty())
        {
            return;
        }

        _saidPhrases.Clear();
        _saidPhrases.Add(await _languageReference.Typed<ILanguageAgent>().SayHello());
        _saidPhrases.Add(await _languageReference.Typed<ILanguageAgent>().SayTest());
        _saidPhrases.Add(await _languageReference.Typed<ILanguageAgent>().SayGoodbye());
    }

    protected override async Task InitialiseNanoReferences()
    {
        await base.InitialiseNanoReferences();

        _languageReference = await CollapseNanoReference(LanguageSplinterIds.LanguageNanoTypeId);
    }

    protected override async Task TerminateNanoReferences()
    {
        await TerminateNanoReference(_languageReference);
    }
}

#endregion

#region Afrikaans

public class AfrikaansLanguageKnowledgeAgent : LanguageKnowledgeAgent
{
    public new static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Afrikaans Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{2FE3B892-576F-42AD-877D-ACBDC63650D4}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region English

public class EnglishLanguageKnowledgeAgent : LanguageKnowledgeAgent
{
    public new static readonly SplinterId NanoInstanceId = new()
    {
        Name = "English Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{9360907D-B9DC-41F7-B3C1-21921A96F04D}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region French

public class FrenchLanguageKnowledgeAgent : LanguageKnowledgeAgent
{
    public new static readonly SplinterId NanoInstanceId = new()
    {
        Name = "French Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{6DFA0E91-D550-42C7-A334-C1BFB5CEE835}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region German

public class GermanLanguageKnowledgeAgent : LanguageKnowledgeAgent
{
    public new static readonly SplinterId NanoInstanceId = new()
    {
        Name = "German Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{016960E7-3AD6-46FF-B7B8-F5C76CBF0193}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion

#region Spanish

public class SpanishLanguageKnowledgeAgent : LanguageKnowledgeAgent
{
    public new static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Spanish Knowledge Agent",
        Version = "1.0.0",
        Guid = new Guid("{7F87ECC3-E859-4E4B-B93D-AEE412506CE5}")
    };

    public override SplinterId InstanceId => NanoInstanceId;
}

#endregion