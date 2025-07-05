namespace UI.Exceptions
{
    public class TourLogNotFoundException : Exception
    {
        public TourLogNotFoundException(int tourLogID) : base($"TourLog with ID {tourLogID} not found") { }
    }
}
