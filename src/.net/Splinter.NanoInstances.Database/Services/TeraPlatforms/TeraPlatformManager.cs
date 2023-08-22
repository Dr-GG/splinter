using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Database.Domain.Settings.TeraPlatforms;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraPlatforms;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.Services.TeraPlatforms;

/// <summary>
/// The default implementation of the ITeraPlatformManager interface.
/// </summary>
public class TeraPlatformManager : ITeraPlatformManager
{
    private readonly TeraPlatformSettings _settings;
    private readonly TeraDbContext _dbContext;
    private readonly IOperatingSystemInformationProvider _osProvider;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraPlatformManager(
        TeraPlatformSettings settings,
        TeraDbContext teraDbContext, 
        IOperatingSystemInformationProvider osProvider)
    {
        _settings = settings;
        _dbContext = teraDbContext;
        _osProvider = osProvider;
    }

    /// <inheritdoc />
    public async Task<long> RegisterTeraPlatform(long operatingSystemId)
    {
        var platform = await GetTeraPlatform();

        if (platform == null)
        {
            platform = await AddNewTeraPlatform(operatingSystemId);
        }
        else
        {
            await SetPlatformAsRunning(platform);
        }

        return platform.Id;
    }

    /// <inheritdoc />
    public async Task DisableTeraPlatform()
    {
        var platform = await GetTeraPlatform();

        if (platform == null)
        {
            return;
        }

        platform.Status = TeraPlatformStatus.Halted;

        await _dbContext.SaveChangesAsync();
    }

    private async Task SetPlatformAsRunning(TeraPlatformModel teraPlatform)
    {
        teraPlatform.Status = TeraPlatformStatus.Running;

        await _dbContext.SaveChangesAsync();
    }

    private async Task<TeraPlatformModel> AddNewTeraPlatform(long operatingSystemId)
    {
        var platformModel = new TeraPlatformModel
        {
            Status = TeraPlatformStatus.Running,
            TeraId = _settings.TeraId,
            FrameworkDescription = await _osProvider.GetFrameworkDescription(),
            OperatingSystemId = operatingSystemId
        };

        await _dbContext.TeraPlatforms.AddAsync(platformModel);
        await _dbContext.SaveChangesAsync();

        return platformModel;
    }

    private async Task<TeraPlatformModel?> GetTeraPlatform()
    {
        return await _dbContext.TeraPlatforms
            .SingleOrDefaultAsync(p => p.TeraId == _settings.TeraId);
    }
}