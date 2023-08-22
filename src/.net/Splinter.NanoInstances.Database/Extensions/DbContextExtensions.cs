using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Exceptions.Data;

namespace Splinter.NanoInstances.Database.Extensions;

/// <summary>
/// Collection of extension methods for the TeraDbContext instance.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Attempts to get a TeraAgentModel based on the Tera Agent ID.
    /// </summary>
    public static async Task<TeraAgentModel> GetTeraAgent(this TeraDbContext teraDbContext, Guid teraId)
    {
        var result = await teraDbContext.TeraAgents
            .SingleOrDefaultAsync(t => t.TeraId == teraId);

        return result ?? throw new SplinterEntityNotFoundException(EntityNameConstants.TeraAgent, teraId);
    }
}