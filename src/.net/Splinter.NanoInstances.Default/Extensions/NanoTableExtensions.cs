using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Extensions
{
    public static class NanoTableExtensions
    {
        public static async Task Register(
            this INanoTable table,
            params INanoReference[] references)
        {
            foreach (var reference in references)
            {
                await table.Register(reference);
            }
        }

        public static async Task Dispose(
            this INanoTable table,
            params INanoReference[] references)
        {
            foreach (var reference in references)
            {
                await table.Dispose(reference);
            }
        }
    }
}
