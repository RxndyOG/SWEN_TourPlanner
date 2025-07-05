namespace UI.Exceptions
{
    public class TourNotFoundException : Exception
    {
        public TourNotFoundException(int tourID) : base($"Tour with ID {tourID} not found") { }
    }
}
