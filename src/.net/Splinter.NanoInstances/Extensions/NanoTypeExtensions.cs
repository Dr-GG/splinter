using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Extensions
{
    public static class NanoTypeExtensions
    {
        public static SplinterId ToSplinterId(this Guid nanoTypeId)
        {
            return new SplinterId
            {
                Name = nanoTypeId.ToString(),
                Version = "0",
                Guid = nanoTypeId
            };
        }
    }
}
