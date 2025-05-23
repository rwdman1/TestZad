using System.ComponentModel.DataAnnotations;

namespace TestZad.Models
{
    /// <summary>
    /// содержит участки (координаты точек контуров сх полей) содержит координаты центра каждого участка
    /// </summary>
    public class LocationInfo
    {
        /// <summary>
        /// [lat, lng]
        /// </summary>
        public double[] Center { get; set; } = new double[2];
        public List<double[]> Polygon { get; set; } = new();

    }
}
