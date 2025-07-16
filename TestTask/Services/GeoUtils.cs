namespace TestTask.Services
{
    public static class GeoUtils
    {
        private const double EarthRadius = 6371000;

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadius * c;
        }

        public static double CalculatePolygonArea(List<(double Lat, double Lng)> polygon)
        {
            double area = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                var (lat1, lon1) = polygon[i];
                var (lat2, lon2) = polygon[(i + 1) % polygon.Count];
                area += DegreesToRadians(lon2 - lon1) *
                        (2 + Math.Sin(DegreesToRadians(lat1)) + Math.Sin(DegreesToRadians(lat2)));
            }

            return Math.Abs(area * EarthRadius * EarthRadius / 2.0);
        }

        public static bool IsPointInPolygon(double lat, double lng, List<(double Lat, double Lng)> polygon)
        {
            int n = polygon.Count;
            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                var xi = polygon[i].Lng;
                var yi = polygon[i].Lat;
                var xj = polygon[j].Lng;
                var yj = polygon[j].Lat;

                bool intersect = ((yi > lat) != (yj > lat)) &&
                                 (lng < (xj - xi) * (lat - yi) / (yj - yi + 1e-12) + xi);
                if (intersect)
                    inside = !inside;
            }

            return inside;
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}
