using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoInstances.Database.Extensions;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Messaging.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Database.Services.Superposition
{
    public class TeraAgentNanoTypeDependencyService : ITeraAgentNanoTypeDependencyService
    {
        private readonly TeraDbContext _dbContext;
        private readonly INanoTypeManager _nanoTypeManager;
        private readonly ITeraAgentManager _teraAgentManager;
        private readonly ITeraMessageRelayService _teraMessageRelayService;

        public TeraAgentNanoTypeDependencyService(
            TeraDbContext dbContext, 
            INanoTypeManager nanoTypeManager, 
            ITeraAgentManager teraAgentManager, 
            ITeraMessageRelayService teraMessageRelayService)
        {
            _dbContext = dbContext;
            _nanoTypeManager = nanoTypeManager;
            _teraAgentManager = teraAgentManager;
            _teraMessageRelayService = teraMessageRelayService;
        }

        public async Task IncrementTeraAgentNanoTypeDependencies(
            Guid teraId, 
            SplinterId nanoType, 
            int numberOfDependencies = 1)
        {
            var nanoTypeId = await _nanoTypeManager.GetNanoTypeId(nanoType);
            var teraAgentId = await _teraAgentManager.GetTeraId(teraId);

            if (teraAgentId == null || nanoTypeId == null)
            {
                return;
            }

            var dependency = await GetDependency(teraAgentId.Value, nanoTypeId.Value)
                             ?? GetDefaultDependency(teraAgentId.Value, nanoTypeId.Value);

            dependency.NumberOfDependencies += numberOfDependencies;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DecrementTeraAgentNanoTypeDependencies(
            Guid teraId, 
            SplinterId nanoType, 
            int numberOfDependencies = 1)
        {
            var nanoTypeId = await _nanoTypeManager.GetNanoTypeId(nanoType);
            var teraAgentId = await _teraAgentManager.GetTeraId(teraId);

            if (teraAgentId == null || nanoTypeId == null)
            {
                return;
            }

            var dependency = await GetDependency(teraAgentId.Value, nanoTypeId.Value);

            if (dependency == null || dependency.NumberOfDependencies == 0)
            {
                return;
            }

            if (numberOfDependencies > dependency.NumberOfDependencies)
            {
                dependency.NumberOfDependencies = 0;
            }
            else
            {
                dependency.NumberOfDependencies -= numberOfDependencies;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DisposeTeraAgentNanoTypeDependencies(Guid teraId)
        {
            var teraAgentId = await _teraAgentManager.GetTeraId(teraId);

            if (teraAgentId == null)
            {
                return;
            }

            var dependency = await GetDependency(teraAgentId.Value);

            if (dependency == null)
            {
                return;
            }

            await _dbContext.TeraAgentNanoTypeDependencies.RemoveAsync(dependency);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Guid?> SignalNanoTypeRecollapses(Guid sourceTeraId, Guid nanoTypeId, IEnumerable<Guid>? teraIds)
        {
            var operation = await GetInitialiseRecollapseOperation(nanoTypeId);

            if (operation == null)
            {
                return null;
            }

            var enumeratedTeraIds = teraIds?.ToList() ?? new List<Guid>();
            var dependencies = (await GetDependencies(nanoTypeId, enumeratedTeraIds)).ToList();
            var recipientTeraIds = dependencies.Select(d => d.TeraAgent.TeraId).ToList();

            if (recipientTeraIds.IsEmpty())
            {
                return null;
            }

            operation.NumberOfExpectedRecollapses = dependencies.Count;

            await SignalNanoTypeRecollapses(nanoTypeId, operation.Guid, sourceTeraId, recipientTeraIds);
            await _dbContext.SaveChangesAsync();

            return operation.Guid;
        }

        private async Task SignalNanoTypeRecollapses(
            Guid nanoTypeId,
            Guid recollapseOperationId,
            Guid sourceTeraId,
            IEnumerable<Guid> teraIds)
        {
            var recollapseData = new RecollapseTeraMessageData
            {
                NanoTypeId = nanoTypeId,
                NanoTypeRecollapseOperationId = recollapseOperationId
            };
            var json = JsonSerializer.Serialize(recollapseData, JsonConstants.DefaultOptions);
            var parameters = new TeraMessageRelayParameters
            {
                Code = TeraMessageCodeConstants.Recollapse,
                Priority = int.MaxValue,
                SourceTeraId = sourceTeraId,
                RecipientTeraIds = teraIds,
                Message = json
            };

            await _teraMessageRelayService.Relay(parameters);
        }

        private async Task<NanoTypeRecollapseOperationModel?> GetInitialiseRecollapseOperation(Guid nanoTypeId)
        {
            var nanoId = await _nanoTypeManager.GetNanoTypeId(nanoTypeId.ToSplinterId());

            if (nanoId == null)
            {
                return null;
            }

            var operation = new NanoTypeRecollapseOperationModel
            {
                NanoTypeId = nanoId.Value,
                Guid = Guid.NewGuid(),
                CreatedTimestamp = DateTime.UtcNow
            };

            await _dbContext.NanoTypeRecollapseOperations.AddAsync(operation);

            return operation;
        }

        private async Task<IEnumerable<TeraAgentNanoTypeDependencyModel>> GetDependencies(
            Guid nanoTypeId,
            IEnumerable<Guid>? teraIds)
        {
            var enumeratedTeraIds = teraIds?.ToList() ?? new List<Guid>();
            var query = _dbContext.TeraAgentNanoTypeDependencies
                .Include(d => d.TeraAgent)
                .Where(d => d.NanoType.Guid == nanoTypeId
                            && d.TeraAgent.Status != TeraAgentStatus.Disposed);

            if (enumeratedTeraIds.Any())
            {
                query = query.Where(d => enumeratedTeraIds.Contains(d.TeraAgent.TeraId));
            }

            return await query.ToListAsync();
        }

        private async Task<TeraAgentNanoTypeDependencyModel?> GetDependency(
            long teraAgentId,
            long? nanoTypeId = null)
        {
            var query = _dbContext.TeraAgentNanoTypeDependencies
                .Where(d => d.TeraAgentId == teraAgentId);

            if (nanoTypeId.HasValue)
            {
                query = query.Where(d => d.NanoTypeId == nanoTypeId);
            }

            return await query.SingleOrDefaultAsync();
        }

        private TeraAgentNanoTypeDependencyModel GetDefaultDependency(
            long teraAgentId,
            long nanoTypeId)
        {
            var model = new TeraAgentNanoTypeDependencyModel
            {
                TeraAgentId = teraAgentId,
                NanoTypeId = nanoTypeId
            };

            _dbContext.TeraAgentNanoTypeDependencies.Add(model);

            return model;
        }
    }
}
