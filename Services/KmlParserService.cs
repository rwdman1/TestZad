using TestZad.Models;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Algorithm;
using System.Xml;
using System.Globalization;
using NetTopologySuite;

namespace TestZad.Services
{
    public class KmlParserService
    {
        private readonly Dictionary<string, Field> _fields = new();
        private readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        /// <summary>
        /// сервис парсинга Kml
        /// </summary>
        public KmlParserService()
        {
            var basePath = Path.Combine(AppContext.BaseDirectory, "Data");
            LoadFields(Path.Combine(basePath, "fields.kml"));
            LoadCentroids(Path.Combine(basePath, "centroids.kml"));
        }
        /// <summary>
        /// загрузка полей при 1 образщении
        /// </summary>
        /// <param name="path"></param>
        public void LoadFields(string path)
        {
            var folder = CheckEx(path);

            foreach (var placemark in folder.Features.OfType<Placemark>())
            {
                var polygon = placemark.Geometry as SharpKml.Dom.Polygon;
                if (polygon == null)
                    continue;

                var coords = polygon.OuterBoundary.LinearRing.Coordinates
                    .Select(c => new[] { c.Latitude, c.Longitude })
                    .ToList();

                var name = placemark.Name;
                var data = placemark.ExtendedData?.SchemaData?.FirstOrDefault().SimpleData;
                var sizeData = data.FirstOrDefault(d => d.Name == "size");
                var id = data.FirstOrDefault(d => d.Name == "fid").Text;
                double.TryParse(sizeData.Text, out var size);

                _fields.Add(id, new Field
                {
                    Id = id,
                    Name = name,
                    Size = size,
                    LocationInfo = new LocationInfo
                    {
                        Polygon = coords
                    }
                });
            }
        }
        /// <summary>
        /// загрузка центров
        /// </summary>
        /// <param name="path"></param>
        private void LoadCentroids(string path)
        {
            var folder = CheckEx(path);
            foreach (var placemark in folder.Features.OfType<Placemark>())
            {
                var id = placemark.ExtendedData?.SchemaData?.FirstOrDefault().SimpleData.FirstOrDefault(d => d.Name == "fid").Text;
                var point = placemark.Geometry as SharpKml.Dom.Point;
                if (point != null && _fields.ContainsKey(id))
                {
                    _fields[id].LocationInfo.Center = [
                    point.Coordinate.Latitude,
                    point.Coordinate.Longitude
                ];
                }
            }
        }
        /// <summary>
        /// общий метод чтения и обработки ошибок
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Folder CheckEx(string path)
        {
            var parser = new Parser();
            using var stream = File.OpenRead(path);
            parser.Parse(stream);

            var kml = parser.Root as Kml;
            var doc = kml?.Feature as Document;
            if (doc == null)
                throw new Exception("Document not found in KML");

            var folder = doc.Features.OfType<Folder>().FirstOrDefault();
            if (folder == null)
                throw new Exception("Folder not found inside Document");
            return folder;
        }

        public List<Field> GetAllFields() => _fields.Values.ToList();

        public Field? GetFieldById(string id) =>
            _fields.TryGetValue(id, out var field) ? field : null;
    }
}
