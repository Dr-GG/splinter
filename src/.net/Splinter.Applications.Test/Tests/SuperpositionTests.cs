using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Splinter.Applications.Test.Agents.NanoAgents.Knowledge;
using Splinter.Applications.Test.Agents.NanoAgents.Knowledge.Languages;
using Splinter.Applications.Test.Agents.NanoAgents.Knowledge.Languages.Phrases;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Domain.Languages;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.Applications.Test.Utilities;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Default.Domain.Enums;
using Splinter.NanoTypes.Default.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.Applications.Test.Tests;

public class SuperpositionTests
{
    public static async Task Test()
    {
        TestUtilities.SectionText("SUPERPOSITION TESTS");

        await TestUtilities.ExecuteTest("Testing default language agent (Afrikaans)", TestDefaultLanguageAgent);
        await TestUtilities.ExecuteTest("Testing recollapse to French hello", TestRecollapseToFrenchHelloAgent);
        await TestUtilities.ExecuteTest("Testing recollapse to French test", TestRecollapseToFrenchTestAgent);
        await TestUtilities.ExecuteTest("Testing recollapse to French goodbye", TestRecollapseToFrenchGoodbyeAgent);
        await TestUtilities.ExecuteTest("Testing recollapse to German phrases", TestRecollapseToAllGermanPhrases);
        await TestUtilities.ExecuteTest("Testing recollapse to Gibberish language", TestRecollapseToGibberishLanguage);
        await TestUtilities.ExecuteTest("Testing recollapse to Japanese knowledge", TestRecollapseToJapaneseKnowledge);
        await TestUtilities.ExecuteTest("Testing recollapse to Afrikaans knowledge (default) #1", TestRecollapseToAfrikaansLanguageAgain);
        await TestUtilities.ExecuteTest("Testing targeted recollapse", TestRecollapseToTargetedTeraAgents);
        await TestUtilities.ExecuteTest("Testing recollapse to Afrikaans knowledge (default) #2", TestRecollapseToAfrikaansLanguageAgain);
        await TestUtilities.ExecuteTest("Testing recollapse to French Hello on multiple targeted agents", TestRecollapseFrenchHelloToMultipleTeraAgents);
        await TestUtilities.ExecuteTest("Testing recollapse to German Hello on all agents", TestRecollapseGermanHelloToAllTeraAgents);
    }

    public static async Task TestDefaultLanguageAgent()
    {
        var afrikaansAgent = await GetTeraLanguageAgent();

        await afrikaansAgent.Execute();

        AssertSaidPhrases(afrikaansAgent, LanguageConstants.Afrikaans);

        await afrikaansAgent.Dispose();
    }

    public static async Task TestRecollapseToFrenchHelloAgent()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent, LanguageConstants.Afrikaans);

        var operationId = await Recollapse<SayFrenchHelloPhraseAgent>(agent);

        await AssertRecollapseOperationCounts(operationId, 1, 0);
        await agent.Execute();
        await AssertRecollapseOperationCounts(operationId, 1, 1);
        AssertSaidPhrases(agent,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToFrenchTestAgent()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        var operationId = await Recollapse<SayFrenchTestPhraseAgent>(agent);

        await AssertRecollapseOperationCounts(operationId, 1, 0);
        await agent.Execute();
        await AssertRecollapseOperationCounts(operationId, 1, 1);
        AssertSaidPhrases(agent,
            LanguageConstants.French.Hello,
            LanguageConstants.French.Test,
            LanguageConstants.Afrikaans.Goodbye);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToFrenchGoodbyeAgent()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent,
            LanguageConstants.French.Hello,
            LanguageConstants.French.Test,
            LanguageConstants.Afrikaans.Goodbye);

        var operationId = await Recollapse<SayFrenchGoodbyePhraseAgent>(agent);

        await AssertRecollapseOperationCounts(operationId, 1, 0);
        await agent.Execute();
        await AssertRecollapseOperationCounts(operationId, 1, 1);
        AssertSaidPhrases(agent, LanguageConstants.French);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToAllGermanPhrases()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent,LanguageConstants.French);

        var helloOperationId = await Recollapse<SayGermanHelloPhraseAgent>(agent);
        var testOperationId = await Recollapse<SayGermanTestPhraseAgent>(agent);
        var goodbyeOperationId = await Recollapse<SayGermanGoodbyePhraseAgent>(agent);

        await AssertRecollapseOperationCounts(helloOperationId, 1, 0);
        await AssertRecollapseOperationCounts(testOperationId, 1, 0);
        await AssertRecollapseOperationCounts(goodbyeOperationId, 1, 0);
        await agent.Execute();
        await AssertRecollapseOperationCounts(helloOperationId, 1, 1);
        await AssertRecollapseOperationCounts(testOperationId, 1, 1);
        await AssertRecollapseOperationCounts(goodbyeOperationId, 1, 1);
        AssertSaidPhrases(agent, LanguageConstants.German);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToGibberishLanguage()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent, LanguageConstants.German);

        var operationId = await Recollapse<GibberishLanguageAgent>(agent);

        await AssertRecollapseOperationCounts(operationId, 1, 0);
        await agent.Execute();
        await AssertRecollapseOperationCounts(operationId, 1, 1);
        AssertSaidPhrases(agent, LanguageConstants.Gibberish);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 0);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToJapaneseKnowledge()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent, LanguageConstants.Gibberish);

        var operationId = await Recollapse<JapaneseKnowledgeAgent>(agent);

        await AssertRecollapseOperationCounts(operationId, 1, 0);
        await agent.Execute();
        await AssertRecollapseOperationCounts(operationId, 1, 1);
        await agent.Execute();
        AssertSaidPhrases(agent, LanguageConstants.Japanese);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 0);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToAfrikaansLanguageAgain()
    {
        var agent = await GetTeraLanguageAgent();

        await agent.Execute();
        AssertSaidPhrases(agent, LanguageConstants.Japanese);

        await Recollapse<SayAfrikaansHelloPhraseAgent>(agent);
        await Recollapse<SayAfrikaansTestPhraseAgent>(agent);
        await Recollapse<SayAfrikaansGoodbyePhraseAgent>(agent);
        await Recollapse<AfrikaansLanguageAgent>(agent);
        var knowledgeOperationId = await Recollapse<AfrikaansLanguageKnowledgeAgent>(agent);

        await AssertRecollapseOperationCounts(knowledgeOperationId, 1, 0);

        await agent.Execute();
        await agent.Execute();
            
        await AssertRecollapseOperationCounts(knowledgeOperationId, 1, 1);
        AssertSaidPhrases(agent, LanguageConstants.Afrikaans);

        await AssertNanoTypeDependency(agent, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent);
    }

    public static async Task TestRecollapseToTargetedTeraAgents()
    {
        var agentToBeFrenchHelloPhrased = await GetTeraLanguageAgent();
        var agentToBeEnglishTestPhrased = await GetTeraLanguageAgent();
        var agentToBeGermanGoodbyePhrased = await GetTeraLanguageAgent();
        var agentToBeGibberishLanguage = await GetTeraLanguageAgent();
        var agentToBeJapaneseKnowledge = await GetTeraLanguageAgent();

        await agentToBeFrenchHelloPhrased.Execute(); AssertSaidPhrases(agentToBeFrenchHelloPhrased, LanguageConstants.Afrikaans);
        await agentToBeEnglishTestPhrased.Execute(); AssertSaidPhrases(agentToBeEnglishTestPhrased, LanguageConstants.Afrikaans);
        await agentToBeGermanGoodbyePhrased.Execute(); AssertSaidPhrases(agentToBeGermanGoodbyePhrased, LanguageConstants.Afrikaans);
        await agentToBeGibberishLanguage.Execute(); AssertSaidPhrases(agentToBeGibberishLanguage, LanguageConstants.Afrikaans);
        await agentToBeJapaneseKnowledge.Execute(); AssertSaidPhrases(agentToBeJapaneseKnowledge, LanguageConstants.Afrikaans);

        var frenchOperationId = await Recollapse<SayFrenchHelloPhraseAgent>(agentToBeFrenchHelloPhrased);
        var englishOperationId = await Recollapse<SayEnglishTestPhraseAgent>(agentToBeEnglishTestPhrased);
        var germanOperationId = await Recollapse<SayGermanGoodbyePhraseAgent>(agentToBeGermanGoodbyePhrased);
        var gibberishOperationId = await Recollapse<GibberishLanguageAgent>(agentToBeGibberishLanguage);
        var japaneseOperationId = await Recollapse<JapaneseKnowledgeAgent>(agentToBeJapaneseKnowledge);

        await AssertRecollapseOperationCounts(frenchOperationId, 1, 0);
        await AssertRecollapseOperationCounts(englishOperationId, 1, 0);
        await AssertRecollapseOperationCounts(germanOperationId, 1, 0);
        await AssertRecollapseOperationCounts(gibberishOperationId, 1, 0);
        await AssertRecollapseOperationCounts(japaneseOperationId, 1, 0);

        await agentToBeFrenchHelloPhrased.Execute();
        await agentToBeEnglishTestPhrased.Execute();
        await agentToBeGermanGoodbyePhrased.Execute();
        await agentToBeGibberishLanguage.Execute();
        await agentToBeJapaneseKnowledge.Execute();
        await agentToBeJapaneseKnowledge.Execute();

        await AssertRecollapseOperationCounts(frenchOperationId, 1, 1);
        await AssertRecollapseOperationCounts(englishOperationId, 1, 1);
        await AssertRecollapseOperationCounts(germanOperationId, 1, 1);
        await AssertRecollapseOperationCounts(gibberishOperationId, 1, 1);
        await AssertRecollapseOperationCounts(japaneseOperationId, 1, 1);

        AssertSaidPhrases(agentToBeFrenchHelloPhrased,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agentToBeEnglishTestPhrased,
            LanguageConstants.Afrikaans.Hello,
            LanguageConstants.English.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agentToBeGermanGoodbyePhrased,
            LanguageConstants.Afrikaans.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.German.Goodbye);

        AssertSaidPhrases(agentToBeGibberishLanguage,LanguageConstants.Gibberish);
        AssertSaidPhrases(agentToBeJapaneseKnowledge, LanguageConstants.Japanese);

        await AssertNanoTypeDependency(agentToBeFrenchHelloPhrased, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeFrenchHelloPhrased, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeFrenchHelloPhrased, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeFrenchHelloPhrased, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeFrenchHelloPhrased, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agentToBeEnglishTestPhrased, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeEnglishTestPhrased, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeEnglishTestPhrased, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeEnglishTestPhrased, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeEnglishTestPhrased, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agentToBeGermanGoodbyePhrased, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeGermanGoodbyePhrased, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeGermanGoodbyePhrased, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeGermanGoodbyePhrased, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeGermanGoodbyePhrased, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agentToBeGibberishLanguage, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeGibberishLanguage, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeGibberishLanguage, SayHelloPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agentToBeGibberishLanguage, SayTestPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agentToBeGibberishLanguage, SayGoodbyePhraseAgent.NanoTypeId, 0);

        await AssertNanoTypeDependency(agentToBeJapaneseKnowledge, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agentToBeJapaneseKnowledge, LanguageAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agentToBeJapaneseKnowledge, SayHelloPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agentToBeJapaneseKnowledge, SayTestPhraseAgent.NanoTypeId, 0);
        await AssertNanoTypeDependency(agentToBeJapaneseKnowledge, SayGoodbyePhraseAgent.NanoTypeId, 0);

        await agentToBeFrenchHelloPhrased.Dispose();
        await agentToBeEnglishTestPhrased.Dispose();
        await agentToBeGermanGoodbyePhrased.Dispose();
        await agentToBeGibberishLanguage.Dispose();
        await agentToBeJapaneseKnowledge.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agentToBeFrenchHelloPhrased);
        await AssertNanoTypeDependenciesAreEmpty(agentToBeEnglishTestPhrased);
        await AssertNanoTypeDependenciesAreEmpty(agentToBeGermanGoodbyePhrased);
        await AssertNanoTypeDependenciesAreEmpty(agentToBeGibberishLanguage);
        await AssertNanoTypeDependenciesAreEmpty(agentToBeJapaneseKnowledge);
    }

    public static async Task TestRecollapseFrenchHelloToMultipleTeraAgents()
    {
        var agent1 = await GetTeraLanguageAgent();
        var agent2 = await GetTeraLanguageAgent();
        var agent3 = await GetTeraLanguageAgent();
        var agent4 = await GetTeraLanguageAgent();
        var agent5 = await GetTeraLanguageAgent();

        await agent1.Execute(); AssertSaidPhrases(agent1, LanguageConstants.Afrikaans);
        await agent2.Execute(); AssertSaidPhrases(agent2, LanguageConstants.Afrikaans);
        await agent3.Execute(); AssertSaidPhrases(agent3, LanguageConstants.Afrikaans);
        await agent4.Execute(); AssertSaidPhrases(agent4, LanguageConstants.Afrikaans);
        await agent5.Execute(); AssertSaidPhrases(agent5, LanguageConstants.Afrikaans);

        var operationId = await Recollapse<SayFrenchHelloPhraseAgent>(new[]
        {
            agent1, agent3, agent5
        });

        await AssertRecollapseOperationCounts(operationId, 3, 0);

        await agent1.Execute();
        await AssertRecollapseOperationCounts(operationId, 3, 1);
        await agent2.Execute();
        await agent3.Execute();
        await AssertRecollapseOperationCounts(operationId, 3, 2);
        await agent4.Execute();
        await agent5.Execute();
        await AssertRecollapseOperationCounts(operationId, 3, 3);

        AssertSaidPhrases(agent1,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent2,
            LanguageConstants.Afrikaans.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent3,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent4,
            LanguageConstants.Afrikaans.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent5,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        await AssertNanoTypeDependency(agent1, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent2, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent3, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent4, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent5, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent1.Dispose();
        await agent2.Dispose();
        await agent3.Dispose();
        await agent4.Dispose();
        await agent5.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent1);
        await AssertNanoTypeDependenciesAreEmpty(agent2);
        await AssertNanoTypeDependenciesAreEmpty(agent3);
        await AssertNanoTypeDependenciesAreEmpty(agent4);
        await AssertNanoTypeDependenciesAreEmpty(agent5);
    }

    public static async Task TestRecollapseGermanHelloToAllTeraAgents()
    {
        var agent1 = await GetTeraLanguageAgent();
        var agent2 = await GetTeraLanguageAgent();
        var agent3 = await GetTeraLanguageAgent();
        var agent4 = await GetTeraLanguageAgent();
        var agent5 = await GetTeraLanguageAgent();

        await agent1.Execute();
        await agent2.Execute();
        await agent3.Execute();
        await agent4.Execute();
        await agent5.Execute();

        AssertSaidPhrases(agent1,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent2,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent3,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent4,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent5,
            LanguageConstants.French.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        var operationId = await Recollapse<SayGermanHelloPhraseAgent>(new[] {agent1}, true);

        await AssertRecollapseOperationCounts(operationId, 5, 0);

        await agent1.Execute();
        await AssertRecollapseOperationCounts(operationId, 5, 1);
        await agent2.Execute();
        await AssertRecollapseOperationCounts(operationId, 5, 2);
        await agent3.Execute();
        await AssertRecollapseOperationCounts(operationId, 5, 3);
        await agent4.Execute();
        await AssertRecollapseOperationCounts(operationId, 5, 4);
        await agent5.Execute();
        await AssertRecollapseOperationCounts(operationId, 5, 5);

        AssertSaidPhrases(agent1,
            LanguageConstants.German.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent2,
            LanguageConstants.German.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent3,
            LanguageConstants.German.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent4,
            LanguageConstants.German.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        AssertSaidPhrases(agent5,
            LanguageConstants.German.Hello,
            LanguageConstants.Afrikaans.Test,
            LanguageConstants.Afrikaans.Goodbye);

        await AssertNanoTypeDependency(agent1, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent1, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent2, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent2, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent3, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent3, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent4, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent4, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await AssertNanoTypeDependency(agent5, LanguageKnowledgeAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, LanguageAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, SayHelloPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, SayTestPhraseAgent.NanoTypeId, 1);
        await AssertNanoTypeDependency(agent5, SayGoodbyePhraseAgent.NanoTypeId, 1);

        await agent1.Dispose();
        await agent2.Dispose();
        await agent3.Dispose();
        await agent4.Dispose();
        await agent5.Dispose();

        await AssertNanoTypeDependenciesAreEmpty(agent1);
        await AssertNanoTypeDependenciesAreEmpty(agent2);
        await AssertNanoTypeDependenciesAreEmpty(agent3);
        await AssertNanoTypeDependenciesAreEmpty(agent4);
        await AssertNanoTypeDependenciesAreEmpty(agent5);
    }

    private static async Task<Guid> Recollapse<TNanoAgent>(ITeraAgent agent) where TNanoAgent : INanoAgent
    {
        return await Recollapse<TNanoAgent>(new[] {agent});
    }

    private static async Task<Guid> Recollapse<TNanoAgent>(IEnumerable<ITeraAgent> agents, bool targetAll = false) where TNanoAgent : INanoAgent
    {
        NanoTypeUtilities.GetSplinterIds(typeof(TNanoAgent), out var nanoTypeId, out _);

        var enumeratedAgents = agents.ToList();
        var superpositionMapping = new SuperpositionMapping
        {
            NanoInstanceType = typeof(TNanoAgent).AssemblyQualifiedName!,
            Scope = SuperpositionScope.Request,
            Mode = SuperpositionMode.Recollapse
        };
        var recollapse = new NanoSuperpositionMappingRecollapseParameters
        {
            SourceTeraId = enumeratedAgents.First().TeraId,
            NanoTypeId = nanoTypeId!.Guid,
            SuperpositionMapping = superpositionMapping,
            TeraIds = targetAll 
                ? Enumerable.Empty<Guid>() 
                : enumeratedAgents.Select(a => a.TeraId)
        };

        var result = await SplinterEnvironment.SuperpositionAgent.Recollapse(recollapse);

        return result ?? Guid.Empty;
    }

    private static void AssertSaidPhrases(ITeraAgent teraAgent, LanguagePack languagePack)
    {
        AssertSaidPhrases(
            teraAgent,
            languagePack.Hello,
            languagePack.Test,
            languagePack.Goodbye);
    }

    private static void AssertSaidPhrases(
        ITeraAgent teraAgent,
        string hello,
        string test,
        string goodbye)
    {
        var knowledge = (ILanguageKnowledgeAgent)teraAgent.Knowledge;
        var phrases = knowledge.SaidPhrases.ToList();

        phrases[0].Should().Be(hello);
        phrases[1].Should().Be(test);
        phrases[2].Should().Be(goodbye);
    }

    private static async Task AssertRecollapseOperationCounts(Guid operationId, 
        int expectedCount, 
        int successfulCount, 
        int failedCount = 0)
    {
        await using var scope = await SplinterEnvironment.SuperpositionAgent.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var operation = dbContext.NanoTypeRecollapseOperations.Single(o => o.Guid == operationId);

        operation.NumberOfExpectedRecollapses.Should().Be(expectedCount);
        operation.NumberOfSuccessfulRecollapses.Should().Be(successfulCount);
        operation.NumberOfFailedRecollapses.Should().Be(failedCount);
    }

    private static async Task AssertNanoTypeDependency(ITeraAgent teraAgent, SplinterId nanoTypeId, int expectedCount)
    {
        await using var scope = await SplinterEnvironment.SuperpositionAgent.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var dependency = await dbContext.TeraAgentNanoTypeDependencies
            .SingleOrDefaultAsync(t => t.TeraAgent.TeraId == teraAgent.TeraId
                                       && t.NanoType.Guid == nanoTypeId.Guid);

        (expectedCount == 0 && (dependency == null 
                                || dependency.NumberOfDependencies == 0)
         || dependency != null && dependency.NumberOfDependencies == expectedCount).Should().BeTrue();
    }

    private static async Task AssertNanoTypeDependenciesAreEmpty(ITeraAgent teraAgent)
    {
        await using var scope = await SplinterEnvironment.SuperpositionAgent.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var hasDependencies = await dbContext.TeraAgentNanoTypeDependencies
            .AnyAsync(t => t.TeraAgent.TeraId == teraAgent.TeraId && t.NumberOfDependencies > 0);

        hasDependencies.Should().BeFalse();
    }

    private static async Task<ITeraAgent> GetTeraLanguageAgent()
    {
        var collapse = new NanoCollapseParameters
        {
            NanoTypeId = LanguageSplinterIds.TeraTypeId.Guid
        };

        var result = (await SplinterEnvironment.SuperpositionAgent.Collapse(collapse) as ITeraAgent)!;

        result.Should().NotBeNull();

        var init = new NanoInitialisationParameters();

        await result.Initialise(init);

        return result;
    }
}