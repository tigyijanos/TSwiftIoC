namespace TSwiftIoC
{
    public class RegistrationKey : IEquatable<RegistrationKey>
    {
        public Type InterfaceType { get; }
        public string? Key { get; }

        public RegistrationKey(Type interfaceType, string? key = null)
        {
            InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            Key = key;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RegistrationKey);
        }

        public bool Equals(RegistrationKey? other)
        {
            return other != null &&
                   InterfaceType == other.InterfaceType &&
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InterfaceType, Key);
        }
    }
}
