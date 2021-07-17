using Splinter.NanoInstances.Tests.Models;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Tests.Mappers
{
    public class RightToLeftMapper : IUnaryMapper<RightModel, LeftModel>
    {
        public LeftModel Map(RightModel source)
        {
            return new()
            {
                Property1 = source.Property1,
                Property2 = source.Property2
            };
        }
    }
}