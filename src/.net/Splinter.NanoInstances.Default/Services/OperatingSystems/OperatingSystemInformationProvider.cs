using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Platforms;

namespace Splinter.NanoInstances.Default.Services.OperatingSystems;

public class OperatingSystemInformationProvider : IOperatingSystemInformationProvider
{
    public Task<OperatingSystemInformation> GetOperatingSystemInformation()
    {
        var osType = GetOperatingSystem();
        var processorArchitecture = GetProcessorArchitecture();
        var osDescription = RuntimeInformation.OSDescription;

        return Task.FromResult(new OperatingSystemInformation
        {
            Type = osType,
            Description = osDescription,
            ProcessorArchitecture = processorArchitecture
        });
    }

    public Task<string> GetFrameworkDescription()
    {
        return Task.FromResult(RuntimeInformation.FrameworkDescription);
    }

    private static ProcessorArchitecture GetProcessorArchitecture()
    {
        return RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.Arm => ProcessorArchitecture.Arm,
            Architecture.Arm64 => ProcessorArchitecture.Arm64,
            Architecture.Wasm => ProcessorArchitecture.Wasm,
            Architecture.X64 => ProcessorArchitecture.x64,
            Architecture.X86 => ProcessorArchitecture.x86,
            _ => ProcessorArchitecture.Unknown
        };
    }

    private static OperatingSystem GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return OperatingSystem.FreeBSD;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OperatingSystem.Linux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OperatingSystem.OSX;
        }

        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? OperatingSystem.Windows 
            : OperatingSystem.Unknown;
    }
}