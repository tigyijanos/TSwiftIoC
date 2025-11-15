using System.Text;

namespace TSwiftIoC
{
    /// <summary>
    /// Exception thrown when a circular dependency is detected during resolution
    /// </summary>
    public class CircularDependencyException : InvalidOperationException
    {
        public CircularDependencyException(string message, Stack<Type> resolutionStack) 
            : base(message)
        {
            ResolutionStack = resolutionStack.ToArray();
        }

        public Type[] ResolutionStack { get; }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(base.Message);
                sb.AppendLine("Resolution stack:");
                foreach (var type in ResolutionStack.Reverse())
                {
                    sb.AppendLine($"  -> {type.Name}");
                }
                return sb.ToString();
            }
        }
    }
}
