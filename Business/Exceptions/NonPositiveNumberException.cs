namespace Business.Exceptions
{
    public class NonPositiveNumberException : Exception
    {
        public NonPositiveNumberException(string attributeName) : base($"Value of {attributeName} must be positive") { }
    }
}
