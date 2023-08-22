using System;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Domain.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Exceptions.Data;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Tenjin.Data.Extensions;

namespace Splinter.NanoInstances.Database.Services.NanoTypes;

/// <summary>
/// The default implementation of the INanoTypeManager interface.
/// </summary>
public class NanoTypeManager : INanoTypeManager
{
    private readonly TeraDbContext _dbContext;
    private readonly INanoTypeCache _cache;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeManager(TeraDbContext teraDbContext, INanoTypeCache cache)
    {
        _dbContext = teraDbContext;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task RegisterNanoType(INanoAgent agent)
    {
        var nanoTypeId = await RegisterNanoTypeId(agent.TypeId);

        await RegisterNanoInstanceId(nanoTypeId, agent.InstanceId);
    }

    /// <inheritdoc />
    public async Task<long?> GetNanoTypeId(SplinterId nanoTypeId)
    {
        if (_cache.TryGetNanoTypeId(nanoTypeId, out var id))
        {
            return id;
        }

        var model = await GetNullableNanoTypeFromDatabase(nanoTypeId);

        return model?.Id;
    }

    /// <inheritdoc />
    public async Task<long?> GetNanoInstanceId(SplinterId nanoInstanceId)
    {
        if (_cache.TryGetNanoInstanceId(nanoInstanceId, out var id))
        {
            return id;
        }

        var model = await GetNullableNanoInstanceFromDatabase(nanoInstanceId);

        return model?.Id;
    }

    private async Task<long> RegisterNanoTypeId(SplinterId nanoTypeId)
    {
        if (_cache.TryGetNanoTypeId(nanoTypeId, out var id))
        {
            return id;
        }

        var nanoTypeModel = await GetNullableNanoTypeFromDatabase(nanoTypeId) 
                            ?? await AddNanoTypeToDatabase(nanoTypeId);

        _cache.RegisterNanoTypeId(nanoTypeId, nanoTypeModel.Id);

        return nanoTypeModel.Id;
    }

    private async Task RegisterNanoInstanceId(long nanoTypeId, SplinterId nanoInstanceId)
    {
        if (_cache.TryGetNanoInstanceId(nanoInstanceId, out _))
        {
            return;
        }

        var nanoInstanceModel = await GetNullableNanoInstanceFromDatabase(nanoInstanceId)
                                ?? await AddNanoInstanceToDatabase(nanoTypeId, nanoInstanceId);

        _cache.RegisterNanoInstanceId(nanoInstanceId, nanoInstanceModel.Id);
    }

    private async Task<NanoTypeModel> GetNonNullableNanoTypeFromDatabase(SplinterId nanoTypeId)
    {
        var result = await GetNullableNanoTypeFromDatabase(nanoTypeId);

        return result ?? throw new SplinterEntityNotFoundException(EntityNameConstants.NanoTypeId, nanoTypeId);
    }

    private async Task<NanoInstanceModel> GetNonNullableNanoInstanceFromDatabase(SplinterId nanoInstanceId)
    {
        var result = await GetNullableNanoInstanceFromDatabase(nanoInstanceId);

        return result ?? throw new SplinterEntityNotFoundException(EntityNameConstants.NanoInstanceId, nanoInstanceId);
    }

    private async Task<NanoTypeModel?> GetNullableNanoTypeFromDatabase(SplinterId nanoTypeId)
    {
        return await _dbContext.NanoTypes
            .SingleOrDefaultAsync(n => n.Guid == nanoTypeId.Guid);
    }

    private async Task<NanoInstanceModel?> GetNullableNanoInstanceFromDatabase(SplinterId nanoInstanceId)
    {
        return await _dbContext.NanoInstances
            .SingleOrDefaultAsync(n => n.Guid == nanoInstanceId.Guid);
    }

    private async Task<NanoTypeModel> AddNanoTypeToDatabase(SplinterId nanoTypeId)
    {
        try
        {
            var nanoTypeModel = new NanoTypeModel
            {
                Guid = nanoTypeId.Guid,
                Name = nanoTypeId.Name,
                Version = nanoTypeId.Version
            };

            await _dbContext.NanoTypes.AddAsync(nanoTypeModel);
            await _dbContext.SaveChangesAsync();

            return nanoTypeModel;
        }
        catch (Exception error)
        {
            if (error.IsDuplicateDataException())
            {
                return await GetNonNullableNanoTypeFromDatabase(nanoTypeId);
            }

            throw;
        }
    }

    private async Task<NanoInstanceModel> AddNanoInstanceToDatabase(long nanoTypeId, SplinterId nanoInstanceId)
    {
        try
        {
            var nanoInstanceModel = new NanoInstanceModel
            {
                NanoTypeId = nanoTypeId,
                Guid = nanoInstanceId.Guid,
                Name = nanoInstanceId.Name,
                Version = nanoInstanceId.Version
            };

            await _dbContext.NanoInstances.AddAsync(nanoInstanceModel);
            await _dbContext.SaveChangesAsync();

            return nanoInstanceModel;
        }
        catch (Exception error)
        {
            if (error.IsDuplicateDataException())
            {
                return await GetNonNullableNanoInstanceFromDatabase(nanoInstanceId);
            }

            throw;
        }
    }
}