using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Exceptions.Data;

namespace Splinter.NanoInstances.Database.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<TeraAgentModel> GetTeraAgent(this TeraDbContext teraDbContext, Guid teraId)
        {
            var result = await teraDbContext.TeraAgents
                .SingleOrDefaultAsync(t => t.TeraId == teraId);

            if (result == null)
            {
                throw new EntityNotFoundException(EntityNameConstants.TeraAgent, teraId);
            }

            return result;
        }

        public static Task RemoveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
            where TEntity : class
        {
            dbSet.Remove(entity);

            return Task.CompletedTask;
        }
    }
}
