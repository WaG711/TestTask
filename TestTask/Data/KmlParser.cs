using System.Globalization;
using System.Xml.Linq;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Data
{
    public static class KmlParser
    {
        public static List<Field> LoadFields(string fieldsPath, string centroidsPath)
        {
            var centroids = ParseCentroids(centroidsPath);
            var fields = new List<Field>();

            var doc = XDocument.Load(fieldsPath);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            foreach (var placemark in doc.Descendants(ns + "Placemark"))
            {
                var id = placemark.Element(ns + "name")?.Value.Trim();
                var coordsText = placemark.Descendants(ns + "coordinates").FirstOrDefault()?.Value.Trim();
                if (coordsText == null) continue;

                var polygon = coordsText
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(coord =>
                    {
                        var parts = coord.Split(',');
                        return (Lat: double.Parse(parts[1], CultureInfo.InvariantCulture), Lng: double.Parse(parts[0], CultureInfo.InvariantCulture));
                    })
                    .ToList();

                centroids.TryGetValue(id, out var center);

                var field = new Field
                {
                    Id = id,
                    Name = id,
                    Center = center,
                    Polygon = polygon,
                    Size = GeoUtils.CalculatePolygonArea(polygon)
                };

                fields.Add(field);
            }

            return fields;
        }

        private static Dictionary<string, (double Lat, double Lng)> ParseCentroids(string path)
        {
            var result = new Dictionary<string, (double, double)>();
            var doc = XDocument.Load(path);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            foreach (var placemark in doc.Descendants(ns + "Placemark"))
            {
                var id = placemark.Element(ns + "name")?.Value.Trim();
                var coord = placemark.Descendants(ns + "coordinates").FirstOrDefault()?.Value.Trim();
                if (coord == null || id == null) continue;

                var parts = coord.Split(',');
                var lng = double.Parse(parts[0], CultureInfo.InvariantCulture);
                var lat = double.Parse(parts[1], CultureInfo.InvariantCulture);
                result[id] = (lat, lng);
            }

            return result;
        }
    }
}
