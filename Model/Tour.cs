namespace Model
{
    public class Tour
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string From_Location { get; set; }
        public string To_Location { get; set; }
        public string Transportation_Type { get; set; }
        public int Distance { get; set; }
        public int Estimated_Time { get; set; }
        public string Route_Information { get; set; }

        public string Image_Path { get; set; }
    }
}
