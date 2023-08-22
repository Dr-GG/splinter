// ReSharper disable InconsistentNaming

namespace Splinter.NanoTypes.Domain.Enums;

/// <summary>
/// Depicts the processor type that a Tera Container is running on.
/// </summary>
public enum ProcessorArchitecture
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 1,

    /// <summary>
    /// 32-bit ARM based architecture.
    /// </summary>
    Arm32 = 2,

    /// <summary>
    /// 64-bit ARM based architecture.
    /// </summary>
    Arm64 = 3,

    /// <summary>
    /// 32-bit x86_32 based architecture.
    /// </summary>
    x86_32 = 4,

    /// <summary>
    /// 64-bit x86_32 based architecture.
    /// </summary>
    x86_64 = 5,

    /// <summary>
    /// Web-assembly based architecture.
    /// </summary>
    Wasm = 6
}