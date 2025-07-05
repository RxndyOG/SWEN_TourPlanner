namespace UI.Exceptions
{
    public class EmptyListException : Exception
    {
        public EmptyListException() : base("List is unexpectedly empty") { }
    }
}
