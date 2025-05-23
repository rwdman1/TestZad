namespace TestZad.Models
{
    public class Field
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        /// <summary>
        /// Площадь в м2
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// центр участка 
        /// </summary>

        public LocationInfo LocationInfo { get; set; } = new();
    }
}
