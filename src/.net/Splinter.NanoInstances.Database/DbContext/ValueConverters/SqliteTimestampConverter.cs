using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Splinter.NanoInstances.Database.DbContext.ValueConverters
{
    public class SqliteTimestampConverter : ValueConverter<byte[], string>
    {
        public SqliteTimestampConverter() : base(
            v => ToDatabase(v),
            v => FromDatabase(v))
        { }

        private static byte[] FromDatabase(string value) =>
            value.Select(c => (byte) c).ToArray();

        private static string ToDatabase(IEnumerable<byte> value) =>
            new(value.Select(b => (char)b).ToArray());
    }
}
