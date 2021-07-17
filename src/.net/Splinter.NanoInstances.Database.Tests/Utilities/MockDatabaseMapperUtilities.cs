using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoInstances.Database.Mappers;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Database.Tests.Utilities
{
    public static class MockDatabaseMapperUtilities
    {
        public static IUnaryMapper<TeraMessageModel, TeraMessage> GetTeraMessageMapper()
        {
            return new TeraMessageMapper();
        }
    }
}
