namespace TSwiftIoC.Enums
{
    public enum Lifetime
    {
        /// <summary>
        /// Creates a new instance every time the service is resolved
        /// </summary>
        PerRequest,
        
        /// <summary>
        /// Creates a single instance that is shared across all resolutions
        /// </summary>
        Singleton,
        
        /// <summary>
        /// Creates one instance per scope (useful for web request scoping)
        /// </summary>
        Scoped
    }
}
