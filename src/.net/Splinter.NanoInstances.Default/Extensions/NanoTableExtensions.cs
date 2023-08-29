using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Extensions;

/// <summary>
/// A collection if INanoTable extension methods.
/// </summary>
public static class NanoTableExtensions
{
    /// <summary>
    /// Registers a collection of INanoReference instances.
    /// </summary>
    public static async Task Register(
        this INanoTable table,
        params INanoReference[] references)
    {
        foreach (var reference in references)
        {
            await table.Register(reference);
        }
    }

    /// <summary>
    /// Removes or deregisters a collection of INanoReference instances.
    /// </summary>
    public static async Task Deregister(
        this INanoTable table,
        params INanoReference[] references)
    {
        foreach (var reference in references)
        {
            await table.Deregister(reference);
        }
    }
}