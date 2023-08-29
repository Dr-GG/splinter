using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Splinter.NanoInstances.Database;
using Splinter.NanoInstances.Default;
using Splinter.NanoInstances.Default.Agents.TeraAgents.Superposition;
using Splinter.NanoInstances.Default.Services.Containers;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Services.ServiceScope;
using Splinter.NanoTypes.Database.Domain.Settings.Databases;
using Splinter.NanoTypes.Default.Domain.Parameters.Bootstrap;
using Splinter.NanoTypes.Default.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Default.Domain.Settings;
using Splinter.NanoTypes.Default.Domain.Settings.SplinterIds;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Domain.Parameters.Bootstrap;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Bootstrap;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Tenjin.Configuration.Extensions;

namespace Splinter.Bootstrap;

/// <summary>
/// The default implementation of the ISplinterBootstrapper interface.
/// </summary>
public class SplinterDefaultBootstrapper : ISplinterBootstrapper
{
    private IServiceScope _serviceScope = null!;

    /// <inheritdoc />
    public async Task Initialise(INanoBootstrapParameters parameters)
    {
        var containerBuilder = new ContainerBuilder();
        var defaultParams = parameters.Cast<NanoDefaultBootstrapParameters>();
        var configuration = GetConfiguration(defaultParams.JsonSettingFileNames);
        var container = AddDefaultModules(configuration, containerBuilder);

        _serviceScope = new AutofacServiceScope(container);

        await InitialiseSplinterEnvironment();
    }

    private static IContainer AddDefaultModules(IConfiguration configuration, ContainerBuilder container)
    {
        var defaultSplinterSettings = configuration.BindObject<SplinterDefaultSettings>("Splinter:Default");
        var databaseSettings = configuration.BindObject<SplinterDatabaseSettings>("Splinter:Database");

        var defaultModule = new NanoInstanceDefaultModule(defaultSplinterSettings);
        var databaseModule = new NanoInstanceDatabaseModule(databaseSettings);

        container.RegisterModule(defaultModule);
        container.RegisterModule(databaseModule);

        return container.Build();
    }

    private async Task InitialiseSplinterEnvironment()
    {
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Initialising;

        var agentIds = await _serviceScope.Resolve<SplinterTeraAgentIdSettings>();

        await InitialiseTeraAgentContainer();
        await InitialiseSuperpositionAgent();
        await InitialiseTeraPlatformAgent();
        await InitialiseTeraRegistryAgent();

        await RegisterCoreTeraAgents(agentIds);

        await InitialiseTeraMessageAgent(agentIds.TeraMessageId);

        SplinterEnvironment.Status = SplinterEnvironmentStatus.Initialised;
    }

    private static async Task RegisterCoreTeraAgents(SplinterTeraAgentIdSettings agentIds)
    {
        await RegisterSelf(SplinterEnvironment.TeraPlatformAgent, agentIds.TeraPlatformId);
        await RegisterSelf(SplinterEnvironment.TeraRegistryAgent, agentIds.TeraRegistryId);
    }

    private static async Task RegisterSelf(ITeraAgent teraAgent, Guid? agentId)
    {
        var parameters = new TeraAgentRegistrationParameters
        {
            TeraId = agentId
        };

        await teraAgent.RegisterSelf(parameters);
    }

    private async Task InitialiseSuperpositionAgent()
    {
        var superpositionAgent = new SuperpositionAgent();
        var settings = await _serviceScope.Resolve<SplinterDefaultSettings>();
        var parameters = new SuperpositionInitialisationParameters
        {
            ServiceScope = await _serviceScope.Start(),
            Register = false,
            SuperpositionMappings = settings.Superposition.Mappings
        };

        await superpositionAgent.Initialise(parameters);

        SplinterEnvironment.SuperpositionAgent = superpositionAgent;
    }

    private static Task InitialiseTeraAgentContainer()
    {
        SplinterEnvironment.TeraAgentContainer = new TeraAgentContainer();

        return Task.CompletedTask;
    }

    private async Task InitialiseTeraPlatformAgent()
    {
        SplinterEnvironment.TeraPlatformAgent = await Initialise<ITeraPlatformAgent>(SplinterIdConstants.TeraPlatformAgentNanoTypeId);
    }

    private async Task InitialiseTeraRegistryAgent()
    {
        SplinterEnvironment.TeraRegistryAgent = await Initialise<ITeraRegistryAgent>(SplinterIdConstants.TeraRegistryAgentNanoTypeId);
    }

    private async Task InitialiseTeraMessageAgent(Guid? teraId)
    {
        SplinterEnvironment.TeraMessageAgent = await Initialise<ITeraMessageAgent>(SplinterIdConstants.TeraMessageAgentNanoTypeId, teraId);
    }

    private async Task<TTeraAgent> Initialise<TTeraAgent>(SplinterId nanoTypeId, Guid? teraId = null) 
        where TTeraAgent : ITeraAgent
    {
        var initParameters = new NanoInitialisationParameters
        {
            Register = teraId.HasValue,
            TeraId = teraId,
            ServiceScope = await _serviceScope.Start()
        };
        var collapseParameters = new NanoCollapseParameters
        {
            NanoTypeId = nanoTypeId.Guid
        };
        var agent = await SplinterEnvironment.SuperpositionAgent.Collapse(collapseParameters) 
                    ?? throw new InvalidNanoTypeException($"Could not load the core singleton nano type {nanoTypeId.Guid}.");

        await agent.Initialise(initParameters);

        return (TTeraAgent)agent;
    }

    private static IConfiguration GetConfiguration(IEnumerable<string> jsonFileNames)
    {
        var builder = new ConfigurationBuilder();

        foreach (var file in jsonFileNames)
        {
            builder.AddJsonFile(file);
        }

        return builder
            .AddEnvironmentVariables()
            .Build();
    }
}