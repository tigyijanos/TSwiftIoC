namespace TSwiftIoC
{
    /// <summary>
    /// Optimized struct-based registration key for high-performance lookups
    /// </summary>
    public readonly struct RegistrationKey : IEquatable<RegistrationKey>
    {
        public Type InterfaceType { get; }
        public string? Key { get; }
        private readonly int _hashCode;

        public RegistrationKey(Type interfaceType, string? key = null)
        {
            InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            Key = key;
            // Pre-calculate hash code for faster lookups
            _hashCode = HashCode.Combine(interfaceType, key);
        }

        public override bool Equals(object? obj)
        {
            return obj is RegistrationKey other && Equals(other);
        }

        public bool Equals(RegistrationKey other)
        {
            // Fast path: compare hash codes first
            return _hashCode == other._hashCode &&
                   InterfaceType == other.InterfaceType &&
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(RegistrationKey left, RegistrationKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RegistrationKey left, RegistrationKey right)
        {
            return !left.Equals(right);
        }
    }
}
