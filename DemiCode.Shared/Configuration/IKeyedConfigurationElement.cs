namespace DemiCode.Configuration
{
    /// <summary>
    /// This interface is intended for use by DemiCode internally.
    /// </summary>
    public interface IKeyedConfigurationElement
    {
        /// <summary>
        /// The name of the element.
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// The key of the element corresponding to <see cref="ElementName"/>.
        /// </summary>
        string ElementKey { get; }
    }
}
