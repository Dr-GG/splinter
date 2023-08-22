using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;

namespace Splinter.NanoInstances.Default.Services.Superposition;

/// <summary>
/// The default implementation of the ISuperpositionMappingResolved interface based on a JSON file.
/// </summary>
public class JsonSuperpositionMappingResolver : ISuperpositionMappingResolver
{
    /// <inheritdoc />
    public Task<IEnumerable<InternalSuperpositionMapping>> Resolve(
        IEnumerable<SuperpositionMapping> superpositionMappings)
    {
        return Task.FromResult(superpositionMappings.Select(GetInternalMapping));
    }

    private static InternalSuperpositionMapping GetInternalMapping(SuperpositionMapping mapping)
    {
        var type = Type.GetType(mapping.NanoInstanceType) 
                   ?? throw new InvalidNanoTypeException($"Could not find the nano type {mapping.NanoInstanceType}.");

        NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out _);

        if (nanoTypeId == null)
        {
            throw new InvalidNanoTypeException(
                $"The nano type {type.FullName} does not consist of a static '{SplinterIdConstants.NanoTypeIdPropertyName}' field.");
        }

        return new InternalSuperpositionMapping
        {
            Mode = mapping.Mode,
            Scope = mapping.Scope,
            Description = mapping.Description,
            NanoInstanceType = mapping.NanoInstanceType,
            NanoTypeId = nanoTypeId.Guid
        };
    }
}