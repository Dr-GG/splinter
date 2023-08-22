namespace Splinter.NanoTypes.Database.Interfaces.Services.Messaging;

/// <summary>
/// The service that is responsible for removing expired TeraMessages.
/// </summary>
public interface IExpiredTeraMessageDisposeService
{
    /// <summary>
    /// Removes and disposes all expired messages.
    /// </summary>
    void DisposeExpiredMessages();
}