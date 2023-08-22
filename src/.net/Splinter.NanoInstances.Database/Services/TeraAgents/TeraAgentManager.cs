﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

namespace Splinter.NanoInstances.Database.Services.TeraAgents;

/// <summary>
/// The default implementation of the ITeraAgentManager interface.
/// </summary>
public class TeraAgentManager : ITeraAgentManager
{
    private readonly TeraDbContext _dbContext;
    private readonly ITeraAgentCache _cache;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraAgentManager(
        TeraDbContext dbContext, 
        ITeraAgentCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<long?> GetTeraId(Guid teraId)
    {
        if (_cache.TryGetTeraId(teraId, out var result))
        {
            return result;
        }

        var id = await GetTeraAgentId(teraId);

        if (id.HasValue)
        {
            _cache.RegisterTeraId(teraId, id.Value);
        }

        return id;
    }

    /// <inheritdoc />
    public Task RegisterTeraId(Guid teraId, long id)
    {
        _cache.RegisterTeraId(teraId, id);

        return Task.CompletedTask;
    }

    private async Task<long?> GetTeraAgentId(Guid teraId)
    {
        var id = await _dbContext.TeraAgents
            .Where(t => t.TeraId == teraId)
            .Select(t => t.Id)
            .SingleOrDefaultAsync();

        return id == 0 ? null : id;
    }
}