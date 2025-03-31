namespace Model
{
    public class TourLog
    {
        public int Id { get; set; }
        public int Tour_Id { get; set; }
        public DateTime Logdate { get; set; }
        public string Comment { get; set; }
        public int Difficulty { get; set; }
        public int Total_Distance { get; set; }
        public int Total_Time { get; set; }
        public int Rating { get; set; }
    }
}
