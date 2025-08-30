namespace DemiCode.Configuration
{
    /// <summary>
    /// Interface used with configuration section managers to handle state of serialization.
    /// </summary>
    public interface ISerializationStateProvider
    {
        /// <summary>
        /// Return true if the section manager is in a serializing mode, false if not.
        /// </summary>
        bool IsSerializing { get; }
    }
}
