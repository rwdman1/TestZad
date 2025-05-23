using Microsoft.AspNetCore.Mvc;
using TestZad.Models;
using TestZad.Services;
using NetTopologySuite.Geometries;

namespace TestZad.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly KmlParserService _kmlService;

        public FieldsController(KmlParserService kmlService)
        {
            _kmlService = kmlService;
        }

        [HttpGet]
        public IActionResult GetAllFields() =>
            Ok(_kmlService.GetAllFields());

        [HttpGet("GetSize/{id}")]
        public IActionResult GetFieldSize(string id)
        {
            var field = _kmlService.GetFieldById(id);
            return field == null ? NotFound() : Ok(field.Size);
        }

        [HttpGet("GetDist/{id}_{lat}_{lng}")]
        public IActionResult GetDistanceToPoint(string id, double lat, double lng)
        {
            var field = _kmlService.GetFieldById(id);
            if (field == null)
                return NotFound();

            var dist = GeometryCalculator.Distance(field.LocationInfo.Center, new[] { lat, lng });
            return Ok(dist);
        }

        [HttpGet("Contains/{lat}_{lng}")]
        public IActionResult PointInField( double lat,  double lng)
        {
            var fields = _kmlService.GetAllFields();
            foreach (var field in fields)
            {
                var coords = field.LocationInfo.Polygon.Select(p => new Coordinate(p[1], p[0])).ToArray();
                var polygon = new Polygon(new LinearRing(coords));
                if (GeometryCalculator.IsPointInsidePolygon(polygon, new[] { lat, lng }))
                {
                    return Ok(new { field.Id, field.Name });
                }
            }
            return Ok(false);
        }
    }
}
