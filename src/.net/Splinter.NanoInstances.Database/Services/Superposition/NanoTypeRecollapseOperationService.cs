using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoInstances.Database.Services.Superposition;

/// <summary>
/// The default implementation of the INanoTypeRecollapseOperationService interface.
/// </summary>
public class NanoTypeRecollapseOperationService : INanoTypeRecollapseOperationService
{
    private readonly TeraDbContext _dbContext;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeRecollapseOperationService(TeraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task Sync(NanoRecollapseOperationParameters parameters)
    {
        var operation = await GetOperation(parameters.NanoRecollapseOperationId);

        if (operation == null)
        {
            return;
        }

        if (parameters.IsSuccessful)
        {
            operation.NumberOfSuccessfulRecollapses++;
        }
        else
        {
            operation.NumberOfFailedRecollapses++;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<NanoTypeRecollapseOperationModel?> GetOperation(Guid id)
    {
        return await _dbContext
            .NanoTypeRecollapseOperations
            .Include(n => n.NanoType)
            .SingleOrDefaultAsync(o => o.Guid == id);
    }
}