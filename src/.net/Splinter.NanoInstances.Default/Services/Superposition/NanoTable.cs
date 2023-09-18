using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Services.Superposition;

/// <summary>
/// The default implementation of the INanoTable interface.
/// </summary>
public class NanoTable : INanoTable
{
    private readonly object _lock = new();
    private readonly IDictionary<Guid, IList<INanoReference>> _references = new Dictionary<Guid, IList<INanoReference>>();

    /// <inheritdoc />
    public Task Register(INanoReference reference)
    {
        if (reference.HasNoReference)
        {
            return Task.CompletedTask;
        }

        var nanoTypeId = reference.Reference.TypeId.Guid;
        var references = GetCollection(nanoTypeId);

        AddNanoReference(references, reference);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<INanoReference>> Fetch(Guid nanoTypeId)
    {
        var collection = GetCollection(nanoTypeId);

        return Task.FromResult(GetNanoReferences(collection, nanoTypeId));
    }

    /// <inheritdoc />
    public Task Deregister(INanoReference reference)
    {
        if (reference.HasNoReference)
        {
            return Task.CompletedTask;
        }

        var nanoTypeId = reference.Reference.TypeId.Guid;
        var collection = GetCollection(nanoTypeId);

        RemoveNanoReference(collection, reference);

        return Task.CompletedTask;
    }

    private IEnumerable<INanoReference> GetNanoReferences(
        IEnumerable<INanoReference> references,
        Guid nanoTypeId)
    {
        lock (_lock)
        {
            return references
                .Where(r => r.HasReference
                            && r.Reference.TypeId.Guid == nanoTypeId)
                .ToList();
        }
    }

    private void AddNanoReference(ICollection<INanoReference> references, INanoReference reference)
    {
        lock (_lock)
        {
            var existingReference = references
                .SingleOrDefault(r => ReferenceEquals(r, reference));

            if (existingReference == null)
            {
                references.Add(reference);
            }
        }
    }

    private void RemoveNanoReference(IList<INanoReference> references, INanoReference reference)
    {
        lock (_lock)
        {
            var data = references
                .Select((r, index) => new
                {
                    Index = index,
                    Reference = r
                })
                .SingleOrDefault(d => ReferenceEquals(d.Reference, reference));

            if (data == null)
            {
                return;
            }

            references.RemoveAt(data.Index);
        }
    }

    private IList<INanoReference> GetCollection(Guid nanoTypeId)
    {
        lock (_lock)
        {
            if (_references.TryGetValue(nanoTypeId, out var result))
            {
                return result;
            }

            _references[nanoTypeId] = new List<INanoReference>();

            return _references[nanoTypeId];
        }
    }
}