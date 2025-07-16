namespace TestTask.Models
{
    public class Field
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public List<(double Lat, double Lng)> Polygon { get; set; }
        public (double Lat, double Lng) Center { get; set; }
    }
}
