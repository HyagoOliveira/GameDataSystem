namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Static factory class for <see cref="ICloudProvider"/> instances.
    /// </summary>
    public static class CloudProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ICloudProvider"/> based on the given type.
        /// </summary>
        /// <param name="type">The Cloud Provider type to use.</param>
        /// <returns>A Cloud Provider implementation.</returns>
        public static ICloudProvider Create(CloudProviderType type) => type switch
        {
            CloudProviderType.None => null,
            CloudProviderType.UnityCloud => new UnityCloudProvider(),
            _ => null
        };
    }
}
