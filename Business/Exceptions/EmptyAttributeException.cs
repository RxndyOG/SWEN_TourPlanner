namespace Business.Exceptions
{
    public class EmptyAttributeException : Exception
    {
        public EmptyAttributeException(string attributeName) : base($"Value of {attributeName} can not be empty") { }
    }
}
