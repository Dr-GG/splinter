using System.Threading.Tasks;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Services.Superposition;

/// <summary>
/// The default implementation of the INanoReference interface.
/// </summary>
public class NanoReference : INanoReference
{
    private INanoAgent? _reference;

    /// <inheritdoc />
    public bool HasNoReference => _reference == null;

    /// <inheritdoc />
    public bool HasReference => _reference != null;

    /// <inheritdoc />
    public INanoAgent Reference => _reference.AssertReturnGetterValue(
        () => new InvalidNanoInstanceException("No nano instance reference initialised."));

    /// <inheritdoc />
    public Task Initialise(INanoAgent? reference)
    {
        _reference = reference;

        return Task.CompletedTask;
    }
}