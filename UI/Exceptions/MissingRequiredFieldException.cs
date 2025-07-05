namespace UI.Exceptions
{
    public class MissingRequiredFieldException : Exception
    {
        public MissingRequiredFieldException() : base("Required field is missing") { }
    }
}
