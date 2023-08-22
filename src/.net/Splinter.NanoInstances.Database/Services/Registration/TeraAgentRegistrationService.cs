using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Database.Interfaces.Services.Registration;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.Data;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoInstances.Database.Services.Registration;

/// <summary>
/// The default implementation of the ITeraAgentRegistrationService interface.
/// </summary>
public class TeraAgentRegistrationService : ITeraAgentRegistrationService
{
    private readonly TeraDbContext _dbContext;
    private readonly INanoTypeManager _nanoTypeManager;
    private readonly ITeraAgentManager _teraAgentManager;
    private readonly ITeraAgentNanoTypeDependencyService _teraAgentNanoTypeDependencyService;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraAgentRegistrationService(
        TeraDbContext teraDbContext,
        INanoTypeManager nanoTypeManager, 
        ITeraAgentManager teraAgentManager, 
        ITeraAgentNanoTypeDependencyService teraAgentNanoTypeDependencyService)
    {
        _dbContext = teraDbContext;
        _nanoTypeManager = nanoTypeManager;
        _teraAgentManager = teraAgentManager;
        _teraAgentNanoTypeDependencyService = teraAgentNanoTypeDependencyService;
    }

    /// <inheritdoc />
    public async Task<Guid> Register(long teraPlatformId, TeraAgentRegistrationParameters parameters)
    {
        if (parameters.TeraId == null)
        {
            return await RegisterNewTeraAgent(teraPlatformId, parameters);
        }

        await RegisterTeraAgentWithId(teraPlatformId, parameters);

        return parameters.TeraId.Value;
    }

    /// <inheritdoc />
    public async Task Dispose(TeraAgentDisposeParameters parameters)
    {
        var agent = await GetTeraAgent(parameters.TeraId);

        if (agent == null)
        {
            return;
        }

        agent.Status = TeraAgentStatus.Disposed;

        await _dbContext.SaveChangesAsync();
    }

    private async Task<Guid> RegisterNewTeraAgent(
        long teraPlatformId,
        TeraAgentRegistrationParameters parameters)
    {
        var teraId = Guid.NewGuid();
        var teraAgent = await AddInitialTeraAgentModel(teraPlatformId, teraId);

        await ConfigureTeraAgent(teraAgent, parameters);
        await _dbContext.SaveChangesAsync();
        await _teraAgentManager.RegisterTeraId(teraId, teraAgent.Id);

        return teraId;
    }

    private async Task RegisterTeraAgentWithId(
        long teraPlatformId, 
        TeraAgentRegistrationParameters parameters)
    {
        if (parameters.TeraId == null)
        {
            return;
        }

        var teraAgent = await GetTeraAgent(parameters.TeraId.Value) 
                        ?? await AddInitialTeraAgentModel(teraPlatformId, parameters.TeraId.Value);

        await ConfigureTeraAgent(teraAgent, parameters);
        await _dbContext.SaveChangesAsync();
        await _teraAgentManager.RegisterTeraId(parameters.TeraId.Value, teraAgent.Id);
        await _teraAgentNanoTypeDependencyService.DisposeTeraAgentNanoTypeDependencies(parameters.TeraId.Value);
    }

    private async Task<TeraAgentModel> AddInitialTeraAgentModel(long teraPlatformId, Guid teraId)
    {
        var model = new TeraAgentModel
        {
            TeraId = teraId,
            TeraPlatformId = teraPlatformId
        };

        await _dbContext.TeraAgents.AddAsync(model);

        return model;
    }

    private async Task ConfigureTeraAgent(
        TeraAgentModel model, 
        TeraAgentRegistrationParameters parameters)
    {
        var nanoInstanceId = await _nanoTypeManager.GetNanoInstanceId(parameters.NanoInstanceId) 
                             ?? throw new SplinterEntityNotFoundException(EntityNameConstants.NanoInstanceId, parameters.NanoInstanceId);

        model.NanoInstanceId = nanoInstanceId;
        model.Status = parameters.TeraAgentStatus;
    }

    private async Task<TeraAgentModel?> GetTeraAgent(Guid teraId)
    {
        return await _dbContext.TeraAgents
            .SingleOrDefaultAsync(t => t.TeraId == teraId);
    }
}