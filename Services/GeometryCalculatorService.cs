using NetTopologySuite.Geometries;

namespace TestZad.Services
{
    /// <summary>
    /// калькулятор геометрии (упрощенный)
    /// </summary>
    public class GeometryCalculator
    {
        /// <summary>
        /// Дистанция, но в методе не совсем уверен. В геометрии я не сильно силен как и в географии
        /// </summary>
        /// <returns></returns>
        public static double Distance(double[] from, double[] to)
        {
            const double R = 6371000; // радиус Земли в метрах

            double lat1 = from[0] * Math.PI / 180;
            double lon1 = from[1] * Math.PI / 180;
            double lat2 = to[0] * Math.PI / 180;
            double lon2 = to[1] * Math.PI / 180;

            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            return 2 * R * Math.Asin(Math.Sqrt(a));
        }

        /// <summary>
        /// Проверяет находится ли точка в полигоне
        /// </summary>
        /// <returns></returns>
        public static bool IsPointInsidePolygon(Polygon polygon, double[] point)
        {
            var coord = new Coordinate(point[0], point[1]);
            var pointGeom = new Point(coord);

            return polygon.Covers(pointGeom);
        }
    }
}
