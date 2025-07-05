namespace UI.Commands
{
    public class DeleteTourMessage
    {
        public int TourId { get; }

        public DeleteTourMessage(int tourId)
        {
            TourId = tourId;
        }
    }
}
