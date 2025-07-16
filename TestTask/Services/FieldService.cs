using TestTask.Data;
using TestTask.Models;

namespace TestTask.Services
{
    public class FieldService
    {
        private readonly List<Field> _fields;

        public FieldService()
        {
            _fields = KmlParser.LoadFields("source/fields.kml", "source/centroids.kml");
        }

        public IEnumerable<object> GetAllFields() => _fields.Select(f => new {
            f.Id,
            f.Name,
            f.Size,
            Locations = new
            {
                Center = new[] { f.Center.Lat, f.Center.Lng },
                Polygon = f.Polygon.Select(p => new[] { p.Lat, p.Lng })
            }
        });

        public double? GetFieldSize(string id) => _fields.FirstOrDefault(f => f.Id == id)?.Size;

        public double? GetDistanceToPoint(string id, double lat, double lng)
        {
            var field = _fields.FirstOrDefault(f => f.Id == id);
            if (field == null) return null;
            return GeoUtils.CalculateDistance(field.Center.Lat, field.Center.Lng, lat, lng);
        }

        public object FindFieldContainingPoint(double lat, double lng)
        {
            foreach (var field in _fields)
            {
                if (GeoUtils.IsPointInPolygon(lat, lng, field.Polygon))
                {
                    return new { field.Id, field.Name };
                }
            }
            return null;
        }
    }
}
