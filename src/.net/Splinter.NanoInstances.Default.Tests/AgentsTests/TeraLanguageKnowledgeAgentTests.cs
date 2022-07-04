using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.Messaging;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoInstances.Default.Tests.Agents.Knowledge;
using Splinter.NanoInstances.Default.Tests.Domain.Constants;
using Splinter.NanoInstances.Default.Tests.Domain.Language;
using Splinter.NanoInstances.Default.Tests.Models;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Messaging.Superposition;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Containers;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Default.Tests.AgentsTests;

[TestFixture]
public class TeraLanguageKnowledgeAgentTests
{
    [SetUp]
    public void Setup()
    {
        InitialiseSplinterEnvironment();
    }

    [TearDown]
    public void Dispose()
    {
        DisposeSplinterEnvironment();
    }

    [Test]
    public async Task Collapse_WhenInitialisingLanguageAgents_InitialisesAsExpected()
    {
        var suite = await InitialiseSuite();

        await Execute(suite);

        AssertSaidPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans);
        AssertSaidPhrases(suite.EnglishAgent, LanguageConstants.English);
        AssertSaidPhrases(suite.FrenchAgent, LanguageConstants.French);
        AssertSaidPhrases(suite.GermanAgent, LanguageConstants.German);
        AssertSaidPhrases(suite.SpanishAgent, LanguageConstants.Spanish);
    }

    [Test]
    public async Task Collapse_WhenInitialisingLanguageAgents_ContainsTheCorrectNanoTableReferences()
    {
        var suite = await InitialiseSuite();

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent);
        await AssertEnglishNanoTable(suite.EnglishAgent);
        await AssertFrenchNanoTable(suite.FrenchAgent);
        await AssertGermanNanoTable(suite.GermanAgent);
        await AssertSpanishNanoTable(suite.SpanishAgent);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToAfrikaans_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesHelloToAfrikaans(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToAfrikaans_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesHelloToAfrikaans(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToEnglish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesHelloToEnglish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToEnglish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesHelloToEnglish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToFrench_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesHelloToFrench(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToFrench_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesHelloToFrench(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToGerman_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesHelloToGerman(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToGerman_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesHelloToGerman(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToSpanish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesHelloToSpanish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingHelloToSpanish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesHelloToSpanish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToAfrikaans_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesTestToAfrikaans(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToAfrikaans_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesTestToAfrikaans(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToEnglish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesTestToEnglish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToEnglish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesTestToEnglish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToFrench_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesTestToFrench(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToFrench_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesTestToFrench(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToGerman_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesTestToGerman(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToGerman_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesTestToGerman(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToSpanish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesTestToSpanish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingTestToSpanish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesTestToSpanish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToAfrikaans_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesGoodbyeToAfrikaans(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToAfrikaans_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesGoodbyeToAfrikaans(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToEnglish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesGoodbyeToEnglish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToEnglish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesGoodbyeToEnglish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToFrench_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesGoodbyeToFrench(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToFrench_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesGoodbyeToFrench(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToGerman_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesGoodbyeToGerman(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToGerman_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesGoodbyeToGerman(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToSpanish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesGoodbyeToSpanish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingGoodbyeToSpanish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesGoodbyeToSpanish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingLanguageAgentToGibberish_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesToGibberish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingLanguageAgentToGibberish_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesLanguageToGibberish(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingLanguageAgentToJapanese_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesToJapanese(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingLanguageAgentToJapanese_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesKnowledgeToJapanese(suite);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingMultipleTimes_RecollapsesAsExpected()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertPhrasesHelloToAfrikaans(suite);
        await RecollapseAndAssertPhrasesHelloToEnglish(suite);
        await RecollapseAndAssertPhrasesHelloToGerman(suite);
        await RecollapseAndAssertPhrasesHelloToSpanish(suite);
        await RecollapseToDefaultAndAssertSaidPhrases(suite, helloNanoTypeId: SayHelloPhraseAgent.NanoTypeId);

        await RecollapseAndAssertPhrasesTestToAfrikaans(suite);
        await RecollapseAndAssertPhrasesTestToEnglish(suite);
        await RecollapseAndAssertPhrasesTestToGerman(suite);
        await RecollapseAndAssertPhrasesTestToSpanish(suite);
        await RecollapseToDefaultAndAssertSaidPhrases(suite, testNanoTypeId: SayTestPhraseAgent.NanoTypeId);

        await RecollapseAndAssertPhrasesGoodbyeToAfrikaans(suite);
        await RecollapseAndAssertPhrasesGoodbyeToEnglish(suite);
        await RecollapseAndAssertPhrasesGoodbyeToGerman(suite);
        await RecollapseAndAssertPhrasesGoodbyeToSpanish(suite);
        await RecollapseToDefaultAndAssertSaidPhrases(suite, goodbyeNanoTypeId: SayGoodbyePhraseAgent.NanoTypeId);

        await RecollapseAndAssertPhrasesToGibberish(suite);
        await RecollapseToDefaultAndAssertSaidPhrases(suite, languageNanoTypeId: LanguageAgents.NanoTypeId);

        await RecollapseAndAssertPhrasesToJapanese(suite);
        await RecollapseToDefaultAndAssertSaidPhrases(suite, LanguageKnowledgeAgent.NanoTypeId, isKnowledgeRecollapse: true);
    }

    [Test]
    public async Task Recollapse_WhenRecollapsingMultipleTimes_ContainsTheCorrectNanoReferences()
    {
        var suite = await InitialiseSuite();

        await RecollapseAndAssertNanoReferencesHelloToAfrikaans(suite);
        await RecollapseAndAssertNanoReferencesHelloToEnglish(suite);
        await RecollapseAndAssertNanoReferencesHelloToGerman(suite);
        await RecollapseAndAssertNanoReferencesHelloToSpanish(suite);
        await RecollapseToDefaultAndAssertNanoReferences(suite, helloNanoTypeId: SayHelloPhraseAgent.NanoTypeId);

        await RecollapseAndAssertNanoReferencesTestToAfrikaans(suite);
        await RecollapseAndAssertNanoReferencesTestToEnglish(suite);
        await RecollapseAndAssertNanoReferencesTestToGerman(suite);
        await RecollapseAndAssertNanoReferencesTestToSpanish(suite);
        await RecollapseToDefaultAndAssertNanoReferences(suite, testNanoTypeId: SayTestPhraseAgent.NanoTypeId);

        await RecollapseAndAssertNanoReferencesGoodbyeToAfrikaans(suite);
        await RecollapseAndAssertNanoReferencesGoodbyeToEnglish(suite);
        await RecollapseAndAssertNanoReferencesGoodbyeToGerman(suite);
        await RecollapseAndAssertNanoReferencesGoodbyeToSpanish(suite);
        await RecollapseToDefaultAndAssertNanoReferences(suite, goodbyeNanoTypeId: SayGoodbyePhraseAgent.NanoTypeId);

        await RecollapseAndAssertNanoReferencesLanguageToGibberish(suite);
        await RecollapseToDefaultAndAssertNanoReferences(suite, languageNanoTypeId: LanguageAgents.NanoTypeId);

        await RecollapseAndAssertNanoReferencesKnowledgeToJapanese(suite);
        await RecollapseToDefaultAndAssertNanoReferences(suite, LanguageKnowledgeAgent.NanoTypeId, isKnowledgeRecollapse: true);
    }

    private static async Task RecollapseToDefaultAndAssertSaidPhrases(
        TeraLanguageAgentsSuite suite,
        SplinterId? knowledgeNanoTypeId = null,
        SplinterId? languageNanoTypeId = null,
        SplinterId? helloNanoTypeId = null,
        SplinterId? testNanoTypeId = null,
        SplinterId? goodbyeNanoTypeId = null,
        bool isKnowledgeRecollapse = false)
    {
        InitialiseRecollapseTeraMessageAgent(
            knowledgeNanoTypeId, languageNanoTypeId,
            helloNanoTypeId, testNanoTypeId, goodbyeNanoTypeId);
        InitialiseDefaultCollapse();

        await Execute(suite);

        if (isKnowledgeRecollapse)
        {
            InitialiseRecollapseTeraMessageAgent();
            await Execute(suite);
        }

        AssertSaidPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans);
        AssertSaidPhrases(suite.EnglishAgent, LanguageConstants.English);
        AssertSaidPhrases(suite.FrenchAgent, LanguageConstants.French);
        AssertSaidPhrases(suite.GermanAgent, LanguageConstants.German);
        AssertSaidPhrases(suite.SpanishAgent, LanguageConstants.Spanish);
    }

    private static async Task RecollapseToDefaultAndAssertNanoReferences(
        TeraLanguageAgentsSuite suite,
        SplinterId? knowledgeNanoTypeId = null,
        SplinterId? languageNanoTypeId = null,
        SplinterId? helloNanoTypeId = null,
        SplinterId? testNanoTypeId = null,
        SplinterId? goodbyeNanoTypeId = null,
        bool isKnowledgeRecollapse = false)
    {
        InitialiseRecollapseTeraMessageAgent(
            knowledgeNanoTypeId, languageNanoTypeId,
            helloNanoTypeId, testNanoTypeId, goodbyeNanoTypeId);
        InitialiseDefaultCollapse();

        await Execute(suite);

        if (isKnowledgeRecollapse)
        {
            InitialiseRecollapseTeraMessageAgent();
            await Execute(suite);
        }

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent);
        await AssertEnglishNanoTable(suite.EnglishAgent);
        await AssertFrenchNanoTable(suite.FrenchAgent);
        await AssertGermanNanoTable(suite.GermanAgent);
        await AssertSpanishNanoTable(suite.SpanishAgent);
    }

    private static async Task RecollapseAndAssertPhrasesHelloToAfrikaans(TeraLanguageAgentsSuite suite)
    {
        InitialiseAfrikaansHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        AssertDifferentHelloPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.Afrikaans);
        AssertDifferentHelloPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.Afrikaans);
        AssertDifferentHelloPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.Afrikaans);
        AssertDifferentHelloPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.Afrikaans);
        AssertDifferentHelloPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.Afrikaans);
    }

    private static async Task RecollapseAndAssertNanoReferencesHelloToAfrikaans(TeraLanguageAgentsSuite suite)
    {
        InitialiseAfrikaansHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayHelloNanoInstanceId: SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayHelloNanoInstanceId: SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayHelloNanoInstanceId: SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayHelloNanoInstanceId: SayAfrikaansHelloPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.EnglishAgent, SayHelloPhraseAgent.NanoTypeId, SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayHelloPhraseAgent.NanoTypeId, SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayHelloPhraseAgent.NanoTypeId, SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayHelloPhraseAgent.NanoTypeId, SaySpanishHelloPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesTestToAfrikaans(TeraLanguageAgentsSuite suite)
    {
        InitialiseAfrikaansTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        AssertDifferentTestPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.Afrikaans);
        AssertDifferentTestPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.Afrikaans);
        AssertDifferentTestPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.Afrikaans);
        AssertDifferentTestPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.Afrikaans);
        AssertDifferentTestPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.Afrikaans);
    }

    private static async Task RecollapseAndAssertNanoReferencesTestToAfrikaans(TeraLanguageAgentsSuite suite)
    {
        InitialiseAfrikaansTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayTestNanoInstanceId: SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayTestNanoInstanceId: SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayTestNanoInstanceId: SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayTestNanoInstanceId: SayAfrikaansTestPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.EnglishAgent, SayTestPhraseAgent.NanoTypeId, SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayTestPhraseAgent.NanoTypeId, SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayTestPhraseAgent.NanoTypeId, SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayTestPhraseAgent.NanoTypeId, SaySpanishTestPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesGoodbyeToAfrikaans(TeraLanguageAgentsSuite suite)
    {
        InitialiseAfrikaansGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        AssertDifferentGoodbyePhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.Afrikaans);
        AssertDifferentGoodbyePhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.Afrikaans);
        AssertDifferentGoodbyePhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.Afrikaans);
        AssertDifferentGoodbyePhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.Afrikaans);
        AssertDifferentGoodbyePhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.Afrikaans);
    }

    private static async Task RecollapseAndAssertNanoReferencesGoodbyeToAfrikaans(TeraLanguageAgentsSuite suite)
    {
        InitialiseAfrikaansGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayGoodbyeNanoInstanceId: SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayGoodbyeNanoInstanceId: SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayGoodbyeNanoInstanceId: SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayGoodbyeNanoInstanceId: SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.EnglishAgent, SayGoodbyePhraseAgent.NanoTypeId, SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayGoodbyePhraseAgent.NanoTypeId, SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayGoodbyePhraseAgent.NanoTypeId, SayGermanGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayGoodbyePhraseAgent.NanoTypeId, SaySpanishGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesHelloToEnglish(TeraLanguageAgentsSuite suite)
    {
        InitialiseEnglishHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        AssertDifferentHelloPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.English);
        AssertDifferentHelloPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.English);
        AssertDifferentHelloPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.English);
        AssertDifferentHelloPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.English);
        AssertDifferentHelloPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.English);
    }

    private static async Task RecollapseAndAssertNanoReferencesHelloToEnglish(TeraLanguageAgentsSuite suite)
    {
        InitialiseEnglishHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayHelloNanoInstanceId: SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayHelloNanoInstanceId: SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayHelloNanoInstanceId: SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayHelloNanoInstanceId: SayEnglishHelloPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayHelloPhraseAgent.NanoTypeId, SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayHelloPhraseAgent.NanoTypeId, SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayHelloPhraseAgent.NanoTypeId, SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayHelloPhraseAgent.NanoTypeId, SaySpanishHelloPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesTestToEnglish(TeraLanguageAgentsSuite suite)
    {
        InitialiseEnglishTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        AssertDifferentTestPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.English);
        AssertDifferentTestPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.English);
        AssertDifferentTestPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.English);
        AssertDifferentTestPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.English);
        AssertDifferentTestPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.English);
    }

    private static async Task RecollapseAndAssertNanoReferencesTestToEnglish(TeraLanguageAgentsSuite suite)
    {
        InitialiseEnglishTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent,
            sayTestNanoInstanceId: SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayTestNanoInstanceId: SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayTestNanoInstanceId: SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayTestNanoInstanceId: SayEnglishTestPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayTestPhraseAgent.NanoTypeId, SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayTestPhraseAgent.NanoTypeId, SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayTestPhraseAgent.NanoTypeId, SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayTestPhraseAgent.NanoTypeId, SaySpanishTestPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesGoodbyeToEnglish(TeraLanguageAgentsSuite suite)
    {
        InitialiseEnglishGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        AssertDifferentGoodbyePhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.English);
        AssertDifferentGoodbyePhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.English);
        AssertDifferentGoodbyePhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.English);
        AssertDifferentGoodbyePhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.English);
        AssertDifferentGoodbyePhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.English);
    }

    private static async Task RecollapseAndAssertNanoReferencesGoodbyeToEnglish(TeraLanguageAgentsSuite suite)
    {
        InitialiseEnglishGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent,
            sayGoodbyeNanoInstanceId: SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayGoodbyeNanoInstanceId: SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayGoodbyeNanoInstanceId: SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayGoodbyeNanoInstanceId: SayEnglishGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayGoodbyePhraseAgent.NanoTypeId, SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayGoodbyePhraseAgent.NanoTypeId, SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayGoodbyePhraseAgent.NanoTypeId, SayGermanGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayGoodbyePhraseAgent.NanoTypeId, SaySpanishGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesHelloToFrench(TeraLanguageAgentsSuite suite)
    {
        InitialiseFrenchHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        AssertDifferentHelloPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.French);
        AssertDifferentHelloPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.French);
        AssertDifferentHelloPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.French);
        AssertDifferentHelloPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.French);
        AssertDifferentHelloPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.French);
    }

    private static async Task RecollapseAndAssertNanoReferencesHelloToFrench(TeraLanguageAgentsSuite suite)
    {
        InitialiseFrenchHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayHelloNanoInstanceId: SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayHelloNanoInstanceId: SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent);
        await AssertGermanNanoTable(suite.GermanAgent, sayHelloNanoInstanceId: SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayHelloNanoInstanceId: SayFrenchHelloPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayHelloPhraseAgent.NanoTypeId, SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayHelloPhraseAgent.NanoTypeId, SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayHelloPhraseAgent.NanoTypeId, SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayHelloPhraseAgent.NanoTypeId, SaySpanishHelloPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesTestToFrench(TeraLanguageAgentsSuite suite)
    {
        InitialiseFrenchTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        AssertDifferentTestPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.French);
        AssertDifferentTestPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.French);
        AssertDifferentTestPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.French);
        AssertDifferentTestPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.French);
        AssertDifferentTestPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.French);
    }

    private static async Task RecollapseAndAssertNanoReferencesTestToFrench(TeraLanguageAgentsSuite suite)
    {
        InitialiseFrenchTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayTestNanoInstanceId: SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayTestNanoInstanceId: SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent);
        await AssertGermanNanoTable(suite.GermanAgent, sayTestNanoInstanceId: SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayTestNanoInstanceId: SayFrenchTestPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayTestPhraseAgent.NanoTypeId, SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayTestPhraseAgent.NanoTypeId, SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayTestPhraseAgent.NanoTypeId, SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayTestPhraseAgent.NanoTypeId, SaySpanishTestPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesGoodbyeToFrench(TeraLanguageAgentsSuite suite)
    {
        InitialiseFrenchGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        AssertDifferentGoodbyePhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.French);
        AssertDifferentGoodbyePhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.French);
        AssertDifferentGoodbyePhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.French);
        AssertDifferentGoodbyePhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.French);
        AssertDifferentGoodbyePhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.French);
    }

    private static async Task RecollapseAndAssertNanoReferencesGoodbyeToFrench(TeraLanguageAgentsSuite suite)
    {
        InitialiseFrenchGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayGoodbyeNanoInstanceId: SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayGoodbyeNanoInstanceId: SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent);
        await AssertGermanNanoTable(suite.GermanAgent, sayGoodbyeNanoInstanceId: SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayGoodbyeNanoInstanceId: SayFrenchGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayGoodbyePhraseAgent.NanoTypeId, SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayGoodbyePhraseAgent.NanoTypeId, SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayGoodbyePhraseAgent.NanoTypeId, SayGermanGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayGoodbyePhraseAgent.NanoTypeId, SaySpanishGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesHelloToGerman(TeraLanguageAgentsSuite suite)
    {
        InitialiseGermanHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        AssertDifferentHelloPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.German);
        AssertDifferentHelloPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.German);
        AssertDifferentHelloPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.German);
        AssertDifferentHelloPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.German);
        AssertDifferentHelloPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.German);
    }

    private static async Task RecollapseAndAssertNanoReferencesHelloToGerman(TeraLanguageAgentsSuite suite)
    {
        InitialiseGermanHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayHelloNanoInstanceId: SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayHelloNanoInstanceId: SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayHelloNanoInstanceId: SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayHelloNanoInstanceId: SayGermanHelloPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayHelloPhraseAgent.NanoTypeId, SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayHelloPhraseAgent.NanoTypeId, SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayHelloPhraseAgent.NanoTypeId, SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayHelloPhraseAgent.NanoTypeId, SaySpanishHelloPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesTestToGerman(TeraLanguageAgentsSuite suite)
    {
        InitialiseGermanTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        AssertDifferentTestPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.German);
        AssertDifferentTestPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.German);
        AssertDifferentTestPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.German);
        AssertDifferentTestPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.German);
        AssertDifferentTestPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.German);
    }

    private static async Task RecollapseAndAssertNanoReferencesTestToGerman(TeraLanguageAgentsSuite suite)
    {
        InitialiseGermanTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayTestNanoInstanceId: SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayTestNanoInstanceId: SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayTestNanoInstanceId: SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayTestNanoInstanceId: SayGermanTestPhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayTestPhraseAgent.NanoTypeId, SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayTestPhraseAgent.NanoTypeId, SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayTestPhraseAgent.NanoTypeId, SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayTestPhraseAgent.NanoTypeId, SaySpanishTestPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesGoodbyeToGerman(TeraLanguageAgentsSuite suite)
    {
        InitialiseGermanGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        AssertDifferentGoodbyePhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.German);
        AssertDifferentGoodbyePhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.German);
        AssertDifferentGoodbyePhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.German);
        AssertDifferentGoodbyePhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.German);
        AssertDifferentGoodbyePhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.German);
    }

    private static async Task RecollapseAndAssertNanoReferencesGoodbyeToGerman(TeraLanguageAgentsSuite suite)
    {
        InitialiseGermanGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayGoodbyeNanoInstanceId: SayGermanGoodbyePhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayGoodbyeNanoInstanceId: SayGermanGoodbyePhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayGoodbyeNanoInstanceId: SayGermanGoodbyePhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent);
        await AssertSpanishNanoTable(suite.SpanishAgent, sayGoodbyeNanoInstanceId: SayGermanGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayGoodbyePhraseAgent.NanoTypeId, SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayGoodbyePhraseAgent.NanoTypeId, SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayGoodbyePhraseAgent.NanoTypeId, SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayGoodbyePhraseAgent.NanoTypeId, SaySpanishGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesHelloToSpanish(TeraLanguageAgentsSuite suite)
    {
        InitialiseSpanishHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        AssertDifferentHelloPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.Spanish);
        AssertDifferentHelloPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.Spanish);
        AssertDifferentHelloPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.Spanish);
        AssertDifferentHelloPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.Spanish);
        AssertDifferentHelloPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.Spanish);
    }

    private static async Task RecollapseAndAssertNanoReferencesHelloToSpanish(TeraLanguageAgentsSuite suite)
    {
        InitialiseSpanishHelloCollapse();
        InitialiseRecollapseTeraMessageAgent(helloNanoTypeId: LanguageSplinterIds.SayHelloNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayHelloNanoInstanceId: SaySpanishHelloPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayHelloNanoInstanceId: SaySpanishHelloPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayHelloNanoInstanceId: SaySpanishHelloPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayHelloNanoInstanceId: SaySpanishHelloPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayHelloPhraseAgent.NanoTypeId, SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayHelloPhraseAgent.NanoTypeId, SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayHelloPhraseAgent.NanoTypeId, SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayHelloPhraseAgent.NanoTypeId, SayGermanHelloPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesTestToSpanish(TeraLanguageAgentsSuite suite)
    {
        InitialiseSpanishTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        AssertDifferentTestPhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.Spanish);
        AssertDifferentTestPhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.Spanish);
        AssertDifferentTestPhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.Spanish);
        AssertDifferentTestPhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.Spanish);
        AssertDifferentTestPhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.Spanish);
    }

    private static async Task RecollapseAndAssertNanoReferencesTestToSpanish(TeraLanguageAgentsSuite suite)
    {
        InitialiseSpanishTestCollapse();
        InitialiseRecollapseTeraMessageAgent(testNanoTypeId: LanguageSplinterIds.SayTestNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayTestNanoInstanceId: SaySpanishTestPhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayTestNanoInstanceId: SaySpanishTestPhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayTestNanoInstanceId: SaySpanishTestPhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayTestNanoInstanceId: SaySpanishTestPhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayTestPhraseAgent.NanoTypeId, SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayTestPhraseAgent.NanoTypeId, SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayTestPhraseAgent.NanoTypeId, SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayTestPhraseAgent.NanoTypeId, SayGermanTestPhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesGoodbyeToSpanish(TeraLanguageAgentsSuite suite)
    {
        InitialiseSpanishGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        AssertDifferentGoodbyePhrases(suite.AfrikaansAgent, LanguageConstants.Afrikaans, LanguageConstants.Spanish);
        AssertDifferentGoodbyePhrases(suite.EnglishAgent, LanguageConstants.English, LanguageConstants.Spanish);
        AssertDifferentGoodbyePhrases(suite.FrenchAgent, LanguageConstants.French, LanguageConstants.Spanish);
        AssertDifferentGoodbyePhrases(suite.GermanAgent, LanguageConstants.German, LanguageConstants.Spanish);
        AssertDifferentGoodbyePhrases(suite.SpanishAgent, LanguageConstants.Spanish, LanguageConstants.Spanish);
    }

    private static async Task RecollapseAndAssertNanoReferencesGoodbyeToSpanish(TeraLanguageAgentsSuite suite)
    {
        InitialiseSpanishGoodbyeCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.SayGoodbyeNanoTypeId);

        await Execute(suite);

        await AssertAfrikaansNanoTable(suite.AfrikaansAgent, sayGoodbyeNanoInstanceId: SaySpanishGoodbyePhraseAgent.NanoInstanceId);
        await AssertEnglishNanoTable(suite.EnglishAgent, sayGoodbyeNanoInstanceId: SaySpanishGoodbyePhraseAgent.NanoInstanceId);
        await AssertFrenchNanoTable(suite.FrenchAgent, sayGoodbyeNanoInstanceId: SaySpanishGoodbyePhraseAgent.NanoInstanceId);
        await AssertGermanNanoTable(suite.GermanAgent, sayGoodbyeNanoInstanceId: SaySpanishGoodbyePhraseAgent.NanoInstanceId);
        await AssertSpanishNanoTable(suite.SpanishAgent);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayGoodbyePhraseAgent.NanoTypeId, SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayGoodbyePhraseAgent.NanoTypeId, SayEnglishGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayGoodbyePhraseAgent.NanoTypeId, SayFrenchGoodbyePhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayGoodbyePhraseAgent.NanoTypeId, SayGermanGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesToGibberish(TeraLanguageAgentsSuite suite)
    {
        InitialiseGibberishCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.LanguageNanoTypeId);

        await Execute(suite);

        AssertSaidPhrases(suite.AfrikaansAgent, LanguageConstants.Gibberish);
        AssertSaidPhrases(suite.EnglishAgent, LanguageConstants.Gibberish);
        AssertSaidPhrases(suite.FrenchAgent, LanguageConstants.Gibberish);
        AssertSaidPhrases(suite.GermanAgent, LanguageConstants.Gibberish);
        AssertSaidPhrases(suite.SpanishAgent, LanguageConstants.Gibberish);
    }

    private static async Task RecollapseAndAssertNanoReferencesLanguageToGibberish(TeraLanguageAgentsSuite suite)
    {
        InitialiseGibberishCollapse();
        InitialiseRecollapseTeraMessageAgent(goodbyeNanoTypeId: LanguageSplinterIds.LanguageNanoTypeId);

        await Execute(suite);

        await AssertSingleNanoTableReference(suite.AfrikaansAgent, LanguageKnowledgeAgent.NanoTypeId, AfrikaansLanguageKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.EnglishAgent, LanguageKnowledgeAgent.NanoTypeId, EnglishLanguageKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.FrenchAgent, LanguageKnowledgeAgent.NanoTypeId, FrenchLanguageKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.GermanAgent, LanguageKnowledgeAgent.NanoTypeId, GermanLanguageKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.SpanishAgent, LanguageKnowledgeAgent.NanoTypeId, SpanishLanguageKnowledgeAgent.NanoInstanceId);

        await AssertSingleNanoTableReference(suite.AfrikaansAgent, LanguageAgents.NanoTypeId, GibberishLanguageAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.EnglishAgent, LanguageAgents.NanoTypeId, GibberishLanguageAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.FrenchAgent, LanguageAgents.NanoTypeId, GibberishLanguageAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.GermanAgent, LanguageAgents.NanoTypeId, GibberishLanguageAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.SpanishAgent, LanguageAgents.NanoTypeId, GibberishLanguageAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayHelloPhraseAgent.NanoTypeId, SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayTestPhraseAgent.NanoTypeId, SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayGoodbyePhraseAgent.NanoTypeId, SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.EnglishAgent, SayHelloPhraseAgent.NanoTypeId, SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayTestPhraseAgent.NanoTypeId, SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayGoodbyePhraseAgent.NanoTypeId, SayEnglishGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.FrenchAgent, SayHelloPhraseAgent.NanoTypeId, SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayTestPhraseAgent.NanoTypeId, SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayGoodbyePhraseAgent.NanoTypeId, SayFrenchGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.GermanAgent, SayHelloPhraseAgent.NanoTypeId, SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayTestPhraseAgent.NanoTypeId, SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayGoodbyePhraseAgent.NanoTypeId, SayGermanGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.SpanishAgent, SayHelloPhraseAgent.NanoTypeId, SaySpanishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayTestPhraseAgent.NanoTypeId, SaySpanishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayGoodbyePhraseAgent.NanoTypeId, SaySpanishGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task RecollapseAndAssertPhrasesToJapanese(TeraLanguageAgentsSuite suite)
    {
        InitialiseJapaneseCollapse();
        InitialiseRecollapseTeraMessageAgent(LanguageSplinterIds.KnowledgeNanoTypeId);

        await Execute(suite);

        InitialiseRecollapseTeraMessageAgent();

        await Execute(suite);

        AssertSaidPhrases(suite.AfrikaansAgent, LanguageConstants.Japanese);
        AssertSaidPhrases(suite.EnglishAgent, LanguageConstants.Japanese);
        AssertSaidPhrases(suite.FrenchAgent, LanguageConstants.Japanese);
        AssertSaidPhrases(suite.GermanAgent, LanguageConstants.Japanese);
        AssertSaidPhrases(suite.SpanishAgent, LanguageConstants.Japanese);
    }

    private static async Task RecollapseAndAssertNanoReferencesKnowledgeToJapanese(TeraLanguageAgentsSuite suite)
    {
        InitialiseJapaneseCollapse();
        InitialiseRecollapseTeraMessageAgent(LanguageSplinterIds.KnowledgeNanoTypeId);

        await Execute(suite);

        InitialiseRecollapseTeraMessageAgent();

        await AssertSingleNanoTableReference(suite.AfrikaansAgent, LanguageKnowledgeAgent.NanoTypeId, JapaneseKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.EnglishAgent, LanguageKnowledgeAgent.NanoTypeId, JapaneseKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.FrenchAgent, LanguageKnowledgeAgent.NanoTypeId, JapaneseKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.GermanAgent, LanguageKnowledgeAgent.NanoTypeId, JapaneseKnowledgeAgent.NanoInstanceId);
        await AssertSingleNanoTableReference(suite.SpanishAgent, LanguageKnowledgeAgent.NanoTypeId, JapaneseKnowledgeAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, LanguageAgents.NanoTypeId, AfrikaansLanguageAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, LanguageAgents.NanoTypeId, EnglishLanguageAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, LanguageAgents.NanoTypeId, FrenchLanguageAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, LanguageAgents.NanoTypeId, GermanLanguageAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, LanguageAgents.NanoTypeId, SpanishLanguageAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayHelloPhraseAgent.NanoTypeId, SayAfrikaansHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayTestPhraseAgent.NanoTypeId, SayAfrikaansTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.AfrikaansAgent, SayGoodbyePhraseAgent.NanoTypeId, SayAfrikaansGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.EnglishAgent, SayHelloPhraseAgent.NanoTypeId, SayEnglishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayTestPhraseAgent.NanoTypeId, SayEnglishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.EnglishAgent, SayGoodbyePhraseAgent.NanoTypeId, SayEnglishGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.FrenchAgent, SayHelloPhraseAgent.NanoTypeId, SayFrenchHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayTestPhraseAgent.NanoTypeId, SayFrenchTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.FrenchAgent, SayGoodbyePhraseAgent.NanoTypeId, SayFrenchGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.GermanAgent, SayHelloPhraseAgent.NanoTypeId, SayGermanHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayTestPhraseAgent.NanoTypeId, SayGermanTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.GermanAgent, SayGoodbyePhraseAgent.NanoTypeId, SayGermanGoodbyePhraseAgent.NanoInstanceId);

        await AssertNoNanoTableReference(suite.SpanishAgent, SayHelloPhraseAgent.NanoTypeId, SaySpanishHelloPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayTestPhraseAgent.NanoTypeId, SaySpanishTestPhraseAgent.NanoInstanceId);
        await AssertNoNanoTableReference(suite.SpanishAgent, SayGoodbyePhraseAgent.NanoTypeId, SaySpanishGoodbyePhraseAgent.NanoInstanceId);
    }

    private static async Task AssertAfrikaansNanoTable(
        INanoAgent teraAgent,
        SplinterId? knowledgeNanoInstanceId = null,
        SplinterId? languageNanoInstanceId = null,
        SplinterId? sayHelloNanoInstanceId = null,
        SplinterId? sayTestNanoInstanceId = null,
        SplinterId? sayGoodbyeNanoInstanceId = null)
    {
        knowledgeNanoInstanceId ??= AfrikaansLanguageKnowledgeAgent.NanoInstanceId;
        languageNanoInstanceId ??= AfrikaansLanguageAgent.NanoInstanceId;
        sayHelloNanoInstanceId ??= SayAfrikaansHelloPhraseAgent.NanoInstanceId;
        sayTestNanoInstanceId ??= SayAfrikaansTestPhraseAgent.NanoInstanceId;
        sayGoodbyeNanoInstanceId ??= SayAfrikaansGoodbyePhraseAgent.NanoInstanceId;

        await AssertSingleNanoTableReference(teraAgent, LanguageKnowledgeAgent.NanoTypeId, knowledgeNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, LanguageAgents.NanoTypeId, languageNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayHelloPhraseAgent.NanoTypeId, sayHelloNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayTestPhraseAgent.NanoTypeId, sayTestNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayGoodbyePhraseAgent.NanoTypeId, sayGoodbyeNanoInstanceId);
    }

    private static async Task AssertEnglishNanoTable(
        INanoAgent teraAgent,
        SplinterId? knowledgeNanoInstanceId = null,
        SplinterId? languageNanoInstanceId = null,
        SplinterId? sayHelloNanoInstanceId = null,
        SplinterId? sayTestNanoInstanceId = null,
        SplinterId? sayGoodbyeNanoInstanceId = null)
    {
        knowledgeNanoInstanceId ??= EnglishLanguageKnowledgeAgent.NanoInstanceId;
        languageNanoInstanceId ??= EnglishLanguageAgent.NanoInstanceId;
        sayHelloNanoInstanceId ??= SayEnglishHelloPhraseAgent.NanoInstanceId;
        sayTestNanoInstanceId ??= SayEnglishTestPhraseAgent.NanoInstanceId;
        sayGoodbyeNanoInstanceId ??= SayEnglishGoodbyePhraseAgent.NanoInstanceId;

        await AssertSingleNanoTableReference(teraAgent, LanguageKnowledgeAgent.NanoTypeId, knowledgeNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, LanguageAgents.NanoTypeId, languageNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayHelloPhraseAgent.NanoTypeId, sayHelloNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayTestPhraseAgent.NanoTypeId, sayTestNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayGoodbyePhraseAgent.NanoTypeId, sayGoodbyeNanoInstanceId);
    }

    private static async Task AssertFrenchNanoTable(
        INanoAgent teraAgent,
        SplinterId? knowledgeNanoInstanceId = null,
        SplinterId? languageNanoInstanceId = null,
        SplinterId? sayHelloNanoInstanceId = null,
        SplinterId? sayTestNanoInstanceId = null,
        SplinterId? sayGoodbyeNanoInstanceId = null)
    {
        knowledgeNanoInstanceId ??= FrenchLanguageKnowledgeAgent.NanoInstanceId;
        languageNanoInstanceId ??= FrenchLanguageAgent.NanoInstanceId;
        sayHelloNanoInstanceId ??= SayFrenchHelloPhraseAgent.NanoInstanceId;
        sayTestNanoInstanceId ??= SayFrenchTestPhraseAgent.NanoInstanceId;
        sayGoodbyeNanoInstanceId ??= SayFrenchGoodbyePhraseAgent.NanoInstanceId;

        await AssertSingleNanoTableReference(teraAgent, LanguageKnowledgeAgent.NanoTypeId, knowledgeNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, LanguageAgents.NanoTypeId, languageNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayHelloPhraseAgent.NanoTypeId, sayHelloNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayTestPhraseAgent.NanoTypeId, sayTestNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayGoodbyePhraseAgent.NanoTypeId, sayGoodbyeNanoInstanceId);
    }

    private static async Task AssertGermanNanoTable(
        INanoAgent teraAgent,
        SplinterId? knowledgeNanoInstanceId = null,
        SplinterId? languageNanoInstanceId = null,
        SplinterId? sayHelloNanoInstanceId = null,
        SplinterId? sayTestNanoInstanceId = null,
        SplinterId? sayGoodbyeNanoInstanceId = null)
    {
        knowledgeNanoInstanceId ??= GermanLanguageKnowledgeAgent.NanoInstanceId;
        languageNanoInstanceId ??= GermanLanguageAgent.NanoInstanceId;
        sayHelloNanoInstanceId ??= SayGermanHelloPhraseAgent.NanoInstanceId;
        sayTestNanoInstanceId ??= SayGermanTestPhraseAgent.NanoInstanceId;
        sayGoodbyeNanoInstanceId ??= SayGermanGoodbyePhraseAgent.NanoInstanceId;

        await AssertSingleNanoTableReference(teraAgent, LanguageKnowledgeAgent.NanoTypeId, knowledgeNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, LanguageAgents.NanoTypeId, languageNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayHelloPhraseAgent.NanoTypeId, sayHelloNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayTestPhraseAgent.NanoTypeId, sayTestNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayGoodbyePhraseAgent.NanoTypeId, sayGoodbyeNanoInstanceId);
    }

    private static async Task AssertSpanishNanoTable(
        INanoAgent teraAgent,
        SplinterId? knowledgeNanoInstanceId = null,
        SplinterId? languageNanoInstanceId = null,
        SplinterId? sayHelloNanoInstanceId = null,
        SplinterId? sayTestNanoInstanceId = null,
        SplinterId? sayGoodbyeNanoInstanceId = null)
    {
        knowledgeNanoInstanceId ??= SpanishLanguageKnowledgeAgent.NanoInstanceId;
        languageNanoInstanceId ??= SpanishLanguageAgent.NanoInstanceId;
        sayHelloNanoInstanceId ??= SaySpanishHelloPhraseAgent.NanoInstanceId;
        sayTestNanoInstanceId ??= SaySpanishTestPhraseAgent.NanoInstanceId;
        sayGoodbyeNanoInstanceId ??= SaySpanishGoodbyePhraseAgent.NanoInstanceId;

        await AssertSingleNanoTableReference(teraAgent, LanguageKnowledgeAgent.NanoTypeId, knowledgeNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, LanguageAgents.NanoTypeId, languageNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayHelloPhraseAgent.NanoTypeId, sayHelloNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayTestPhraseAgent.NanoTypeId, sayTestNanoInstanceId);
        await AssertSingleNanoTableReference(teraAgent, SayGoodbyePhraseAgent.NanoTypeId, sayGoodbyeNanoInstanceId);
    }

    private static async Task AssertSingleNanoTableReference(
        INanoAgent teraAgent,
        SplinterId nanoType,
        SplinterId nanoInstance)
    {
        var references = (await teraAgent.NanoTable.Fetch(nanoType.Guid)).ToList();
        var instance = references.First().Reference;

        Assert.AreEqual(1, references.Count);
        Assert.AreEqual(nanoType, instance.TypeId);
        Assert.AreEqual(nanoInstance, instance.InstanceId);
        Assert.IsTrue(instance.HasTeraParent);
        Assert.IsTrue(instance.HasNanoTable);
    }

    private static async Task AssertNoNanoTableReference(
        INanoAgent teraAgent, 
        SplinterId nanoType,
        SplinterId nanoInstance)
    {
        var references = (await teraAgent.NanoTable.Fetch(nanoType.Guid)).ToList();
        var instances = references.Where(r => r.HasReference && r.Reference.InstanceId.Equals(nanoInstance));

        Assert.IsEmpty(instances);
    }

    private static void AssertDifferentHelloPhrases(
        ITeraAgent teraAgent, 
        LanguagePack originalLanguagePack,
        LanguagePack helloLanguagePack)
    {
        AssertSaidPhrases(
            teraAgent,
            helloLanguagePack.Hello,
            originalLanguagePack.Test,
            originalLanguagePack.Goodbye);
    }

    private static void AssertDifferentTestPhrases(
        ITeraAgent teraAgent,
        LanguagePack originalLanguagePack,
        LanguagePack testLanguagePack)
    {
        AssertSaidPhrases(
            teraAgent,
            originalLanguagePack.Hello,
            testLanguagePack.Test,
            originalLanguagePack.Goodbye);
    }

    private static void AssertDifferentGoodbyePhrases(
        ITeraAgent teraAgent,
        LanguagePack originalLanguagePack,
        LanguagePack goodbyeLanguagePack)
    {
        AssertSaidPhrases(
            teraAgent,
            originalLanguagePack.Hello,
            originalLanguagePack.Test,
            goodbyeLanguagePack.Goodbye);
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
        var knowledge = (ILanguageKnowledgeAgent) teraAgent.Knowledge;
        var phrases = knowledge.SaidPhrases.ToList();

        Assert.AreEqual(hello, phrases[0]);
        Assert.AreEqual(test, phrases[1]);
        Assert.AreEqual(goodbye, phrases[2]);
    }

    private static void InitialiseSplinterEnvironment()
    {
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Initialised;
        SplinterEnvironment.SuperpositionAgent = GetDefaultSuperpositionAgent();
        SplinterEnvironment.TeraRegistryAgent = GetDefaultRegistryAgent();
        SplinterEnvironment.TeraMessageAgent = new Mock<ITeraMessageAgent>().Object;
        SplinterEnvironment.TeraAgentContainer = new Mock<ITeraAgentContainer>().Object;
    }

    private static void InitialiseDefaultCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetDefaultSuperpositionAgent();
    }

    private static void InitialiseAfrikaansHelloCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetAfrikaansHelloSuperpositionAgent();
    }

    private static void InitialiseEnglishHelloCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetEnglishHelloSuperpositionAgent();
    }

    private static void InitialiseFrenchHelloCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetFrenchHelloSuperpositionAgent();
    }

    private static void InitialiseGermanHelloCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetGermanHelloSuperpositionAgent();
    }

    private static void InitialiseSpanishHelloCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetSpanishHelloSuperpositionAgent();
    }

    private static void InitialiseAfrikaansTestCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetAfrikaansTestSuperpositionAgent();
    }

    private static void InitialiseEnglishTestCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetEnglishTestSuperpositionAgent();
    }

    private static void InitialiseFrenchTestCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetFrenchTestSuperpositionAgent();
    }

    private static void InitialiseGermanTestCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetGermanTestSuperpositionAgent();
    }

    private static void InitialiseSpanishTestCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetSpanishTestSuperpositionAgent();
    }

    private static void InitialiseAfrikaansGoodbyeCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetAfrikaansGoodbyeSuperpositionAgent();
    }

    private static void InitialiseEnglishGoodbyeCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetEnglishGoodbyeSuperpositionAgent();
    }

    private static void InitialiseFrenchGoodbyeCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetFrenchGoodbyeSuperpositionAgent();
    }

    private static void InitialiseGermanGoodbyeCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetGermanGoodbyeSuperpositionAgent();
    }

    private static void InitialiseSpanishGoodbyeCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetSpanishGoodbyeSuperpositionAgent();
    }

    private static void InitialiseGibberishCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetGibberishLanguageSuperpositionAgent();
    }

    private static void InitialiseJapaneseCollapse()
    {
        SplinterEnvironment.SuperpositionAgent = GetJapaneseKnowledgeSuperpositionAgent();
    }

    private static void InitialiseRecollapseTeraMessageAgent(
        SplinterId? knowledgeNanoTypeId = null,
        SplinterId? languageNanoTypeId = null,
        SplinterId? helloNanoTypeId = null,
        SplinterId? testNanoTypeId = null,
        SplinterId? goodbyeNanoTypeId = null)
    {
        var counter = 1;
        var result = new Mock<ITeraMessageAgent>();
        var messages = GetRecollapseTeraMessages(
            knowledgeNanoTypeId, languageNanoTypeId,
            helloNanoTypeId, testNanoTypeId, goodbyeNanoTypeId);

        result
            .Setup(r => r.Dequeue(It.IsAny<TeraMessageDequeueParameters>()))
            .ReturnsAsync(() => counter++ % 2 == 1
                ? messages
                : Enumerable.Empty<TeraMessage>());

        SplinterEnvironment.TeraMessageAgent = result.Object;
    }

    private static void DisposeSplinterEnvironment()
    {
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Uninitialised;
        SplinterEnvironment.SuperpositionAgent = GetDefaultSuperpositionAgent();
    }

    private static async Task<TeraLanguageAgentsSuite> InitialiseSuite()
    {
        return new TeraLanguageAgentsSuite
        {
            AfrikaansAgent = await InitialiseLanguageAgent(LanguageSplinterIds.AfrikaansTeraId),
            EnglishAgent = await InitialiseLanguageAgent(LanguageSplinterIds.EnglishTeraId),
            FrenchAgent = await InitialiseLanguageAgent(LanguageSplinterIds.FrenchTeraId),
            GermanAgent = await InitialiseLanguageAgent(LanguageSplinterIds.GermanTeraId),
            SpanishAgent = await InitialiseLanguageAgent(LanguageSplinterIds.SpanishTeraId)
        };
    }

    private static async Task Execute(TeraLanguageAgentsSuite suite)
    {
        await Execute(suite.AfrikaansAgent);
        await Execute(suite.EnglishAgent);
        await Execute(suite.FrenchAgent);
        await Execute(suite.GermanAgent);
        await Execute(suite.SpanishAgent);
    }

    private static async Task Execute(ITeraAgent teraAgent)
    {
        var executeParameters = new TeraAgentExecutionParameters();

        await teraAgent.Execute(executeParameters);
    }

    private static async Task<ITeraAgent> InitialiseLanguageAgent(Guid teraId)
    {
        var parameters = new NanoCollapseParameters
        {
            TeraAgentId = teraId,
            NanoTypeId = LanguageSplinterIds.TeraTypeId.Guid
        };
        var result = await SplinterEnvironment.SuperpositionAgent.Collapse(parameters);

        if (result == null)
        {
            throw new NotSupportedException($"Could not collapse tera id {teraId}");
        }

        var init = new NanoInitialisationParameters
        {
            ServiceScope = GetScope(),
            TeraId = teraId
        };

        await result.Initialise(init);

        return (ITeraAgent) result;
    }

    private static IServiceScope GetScope()
    {
        var result = new Mock<IServiceScope>();

        result
            .Setup(s => s.Resolve<INanoTable>())
            .ReturnsAsync(() => new NanoTable());

        result
            .Setup(s => s.Resolve<ITeraMessageQueue>())
            .ReturnsAsync(() => new TeraMessageQueue(
                new TeraMessagingSettings()));

        result
            .Setup(s => s.Resolve<IRecollapseNanoTypeService>())
            .ReturnsAsync(() => new RecollapseNanoTypeService());

        result
            .Setup(s => s.Start())
            .ReturnsAsync(GetScope);

        return result.Object;
    }

    private static IEnumerable<TeraMessage> GetRecollapseTeraMessages(
        SplinterId? knowledgeNanoTypeId,
        SplinterId? languageNanoTypeId,
        SplinterId? helloNanoTypeId,
        SplinterId? phraseNanoTypeId,
        SplinterId? goodbyeNanoTypeId)
    {
        var result = new List<TeraMessage>();

        if (knowledgeNanoTypeId != null)
        {
            result.Add(GetRecollapseTeraMessage(knowledgeNanoTypeId));
        }

        if (languageNanoTypeId != null)
        {
            result.Add(GetRecollapseTeraMessage(languageNanoTypeId));
        }

        if (helloNanoTypeId != null)
        {
            result.Add(GetRecollapseTeraMessage(helloNanoTypeId));
        }

        if (phraseNanoTypeId != null)
        {
            result.Add(GetRecollapseTeraMessage(phraseNanoTypeId));
        }

        if (goodbyeNanoTypeId != null)
        {
            result.Add(GetRecollapseTeraMessage(goodbyeNanoTypeId));
        }

        return result;
    }

    private static TeraMessage GetRecollapseTeraMessage(SplinterId nanoTypeId)
    {
        var result = new TeraMessage {Code = TeraMessageCodeConstants.Recollapse};
        var data = new RecollapseTeraMessageData {NanoTypeId = nanoTypeId.Guid};

        result.JsonMessage(data);

        return result;
    }

    private static ISuperpositionAgent GetDefaultSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync((NanoCollapseParameters parameters) =>
            {
                if (parameters.NanoTypeId == LanguageSplinterIds.TeraTypeId.Guid)
                {
                    return new LanguageTeraAgent(parameters.TeraAgentId!.Value);
                }

                if (parameters.TeraAgentId.HasValue &&
                    parameters.TeraAgentId == LanguageSplinterIds.AfrikaansTeraId)
                {
                    if (parameters.NanoTypeId == LanguageSplinterIds.KnowledgeNanoTypeId.Guid)
                    {
                        return new AfrikaansLanguageKnowledgeAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.LanguageNanoTypeId.Guid)
                    {
                        return new AfrikaansLanguageAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayHelloNanoTypeId.Guid)
                    {
                        return new SayAfrikaansHelloPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayTestNanoTypeId.Guid)
                    {
                        return new SayAfrikaansTestPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayGoodbyeNanoTypeId.Guid)
                    {
                        return new SayAfrikaansGoodbyePhraseAgent();
                    }
                }

                if (parameters.TeraAgentId.HasValue &&
                    parameters.TeraAgentId == LanguageSplinterIds.EnglishTeraId)
                {
                    if (parameters.NanoTypeId == LanguageSplinterIds.KnowledgeNanoTypeId.Guid)
                    {
                        return new EnglishLanguageKnowledgeAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.LanguageNanoTypeId.Guid)
                    {
                        return new EnglishLanguageAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayHelloNanoTypeId.Guid)
                    {
                        return new SayEnglishHelloPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayTestNanoTypeId.Guid)
                    {
                        return new SayEnglishTestPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayGoodbyeNanoTypeId.Guid)
                    {
                        return new SayEnglishGoodbyePhraseAgent();
                    }
                }

                if (parameters.TeraAgentId.HasValue &&
                    parameters.TeraAgentId == LanguageSplinterIds.FrenchTeraId)
                {
                    if (parameters.NanoTypeId == LanguageSplinterIds.KnowledgeNanoTypeId.Guid)
                    {
                        return new FrenchLanguageKnowledgeAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.LanguageNanoTypeId.Guid)
                    {
                        return new FrenchLanguageAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayHelloNanoTypeId.Guid)
                    {
                        return new SayFrenchHelloPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayTestNanoTypeId.Guid)
                    {
                        return new SayFrenchTestPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayGoodbyeNanoTypeId.Guid)
                    {
                        return new SayFrenchGoodbyePhraseAgent();
                    }
                }

                if (parameters.TeraAgentId.HasValue &&
                    parameters.TeraAgentId == LanguageSplinterIds.GermanTeraId)
                {
                    if (parameters.NanoTypeId == LanguageSplinterIds.KnowledgeNanoTypeId.Guid)
                    {
                        return new GermanLanguageKnowledgeAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.LanguageNanoTypeId.Guid)
                    {
                        return new GermanLanguageAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayHelloNanoTypeId.Guid)
                    {
                        return new SayGermanHelloPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayTestNanoTypeId.Guid)
                    {
                        return new SayGermanTestPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayGoodbyeNanoTypeId.Guid)
                    {
                        return new SayGermanGoodbyePhraseAgent();
                    }
                }

                // ReSharper disable once InvertIf
                if (parameters.TeraAgentId.HasValue &&
                    parameters.TeraAgentId == LanguageSplinterIds.SpanishTeraId)
                {
                    if (parameters.NanoTypeId == LanguageSplinterIds.KnowledgeNanoTypeId.Guid)
                    {
                        return new SpanishLanguageKnowledgeAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.LanguageNanoTypeId.Guid)
                    {
                        return new SpanishLanguageAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayHelloNanoTypeId.Guid)
                    {
                        return new SaySpanishHelloPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayTestNanoTypeId.Guid)
                    {
                        return new SaySpanishTestPhraseAgent();
                    }

                    if (parameters.NanoTypeId == LanguageSplinterIds.SayGoodbyeNanoTypeId.Guid)
                    {
                        return new SaySpanishGoodbyePhraseAgent();
                    }
                }

                return null;
            });

        return result.Object;
    }

    private static ISuperpositionAgent GetAfrikaansHelloSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayAfrikaansHelloPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetEnglishHelloSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayEnglishHelloPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetFrenchHelloSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayFrenchHelloPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetGermanHelloSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayGermanHelloPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetSpanishHelloSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SaySpanishHelloPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetAfrikaansTestSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayAfrikaansTestPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetEnglishTestSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayEnglishTestPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetFrenchTestSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayFrenchTestPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetGermanTestSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayGermanTestPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetSpanishTestSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SaySpanishTestPhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetAfrikaansGoodbyeSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayAfrikaansGoodbyePhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetEnglishGoodbyeSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayEnglishGoodbyePhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetFrenchGoodbyeSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayFrenchGoodbyePhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetGermanGoodbyeSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SayGermanGoodbyePhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetSpanishGoodbyeSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new SaySpanishGoodbyePhraseAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetGibberishLanguageSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new GibberishLanguageAgent());

        return result.Object;
    }

    private static ISuperpositionAgent GetJapaneseKnowledgeSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(() => new JapaneseKnowledgeAgent());

        return result.Object;
    }

    private static ITeraRegistryAgent GetDefaultRegistryAgent()
    {
        var result = new Mock<ITeraRegistryAgent>();

        result
            .Setup(r => r.Register(It.IsAny<TeraAgentRegistrationParameters>()))
            .ReturnsAsync((TeraAgentRegistrationParameters parameters) => parameters.TeraId!.Value);

        return result.Object;
    }
}