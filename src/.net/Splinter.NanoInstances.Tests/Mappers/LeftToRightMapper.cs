using Splinter.NanoInstances.Tests.Models;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Tests.Mappers
{
    public class LeftToRightMapper : IUnaryMapper<LeftModel, RightModel>
    {
        public RightModel Map(LeftModel source)
        {
            return new()
            {
                Property1 = source.Property1,
                Property2 = source.Property2
            };
        }
    }
}
