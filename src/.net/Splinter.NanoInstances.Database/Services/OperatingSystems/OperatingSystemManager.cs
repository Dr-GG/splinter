using System.Linq;
using System.Threading.Tasks;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Domain.Platforms;

namespace Splinter.NanoInstances.Database.Services.OperatingSystems;

/// <summary>
/// Gets the default implementation of the IOperatingSystemManager interface.
/// </summary>
public class OperatingSystemManager : IOperatingSystemManager
{
    private readonly TeraDbContext _dbContext;
    private readonly IOperatingSystemInformationProvider _osProvider;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public OperatingSystemManager(
        TeraDbContext teraDbContext, 
        IOperatingSystemInformationProvider osProvider)
    {
        _dbContext = teraDbContext;
        _osProvider = osProvider;
    }

    /// <inheritdoc />
    public async Task<long> RegisterOperatingSystemInformation()
    {
        var osInformation = await _osProvider.GetOperatingSystemInformation();

        if (OperatingSystemExists(osInformation, out var id))
        {
            return id;
        }

        return await SaveNewOperatingSystem(osInformation);
    }

    private bool OperatingSystemExists(OperatingSystemInformation osInfo, out long osId)
    {
        osId = _dbContext.OperatingSystems
            .Where(o => o.Description == osInfo.Description
                        && o.ProcessorArchitecture == osInfo.ProcessorArchitecture
                        && o.Type == osInfo.Type)
            .Select(o => o.Id)
            .SingleOrDefault();

        return osId != 0;
    }

    private async Task<long> SaveNewOperatingSystem(OperatingSystemInformation osInfo)
    {
        var osModel = new OperatingSystemModel
        {
            Description = osInfo.Description,
            ProcessorArchitecture = osInfo.ProcessorArchitecture,
            Type = osInfo.Type
        };

        await _dbContext.OperatingSystems.AddAsync(osModel);
        await _dbContext.SaveChangesAsync();

        return osModel.Id;
    }
}