using System;
using System.Threading.Tasks;
using Splinter.Applications.Test.Tests;
using Splinter.Applications.Test.Utilities;
using Splinter.Bootstrap;
using Splinter.NanoTypes.Default.Domain.Parameters.Bootstrap;

namespace Splinter.Applications.Test;

public static class Program
{
    private static async Task Main()
    {
        TestUtilities.Reset();
        Console.Write("Starting...");

        var bootstrapper = new SplinterDefaultBootstrapper();
        var parameters = new NanoDefaultBootstrapParameters
        {
            JsonSettingFileNames = new[] {"splinter.settings.json"}
        };

        await bootstrapper.Initialise(parameters);
        Console.WriteLine("DONE!\r\n");
        Console.WriteLine("Executing Splinter integration tests.");

        await TestTeraAgentLifecycleTests.Test();
        await TeraMessageTests.Test();
        await SuperpositionTests.Test();

        TestUtilities.FinalSuccess();
    }
}