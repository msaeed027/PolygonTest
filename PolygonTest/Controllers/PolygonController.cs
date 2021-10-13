using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using PolygonTest.Database;
using System.Collections.Generic;

namespace PolygonTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PolygonController : ControllerBase
    {
        public const int SRID = 4326;
        
        private readonly ILogger<PolygonController> _logger;
        private readonly MyDbContext _myDbContext;



        public PolygonController(ILogger<PolygonController> logger, MyDbContext myDbContext)
        {
            _logger = logger;
            _myDbContext = myDbContext;
        }

        [HttpGet]
        public ResultDto Get()
        {
            /*
            var coords = new List<Coordinate>()
            {
                new Coordinate(39.81591, 21.525905),
                new Coordinate(39.802169, 21.584658),
                new Coordinate(39.744457, 21.591043),
                new Coordinate(39.75545, 21.52335),
                new Coordinate(39.729342, 21.50163),
                new Coordinate(39.711479, 21.435173),
                new Coordinate(39.666133, 21.436452),
                new Coordinate(39.7101, 21.353339),
                new Coordinate(39.798043, 21.32264),
                new Coordinate(40.008967, 21.295132),
                new Coordinate(40.052287, 21.359734),
                new Coordinate(39.989079, 21.511851),
                new Coordinate(39.886021, 21.538679),
                new Coordinate(39.81591, 21.525905)
            };
        
            Polygon polygon = new Polygon(new LinearRing(coords.ToArray())) { SRID = SRID };

            _myDbContext.User.Add(new User
            {
                PlacePoly = polygon
            });
            _myDbContext.SaveChanges();
            */

            var user = _myDbContext.Find<User>(3);
            var polygon = user.PlacePoly;

            // polygon.AsText(); // well known text for spatial shapes: POLYGON((39.81591 21.525905), (39.802169 21.584658))
            // polygon.AsBinary(); // well known binary

            string wkt = polygon.AsText();

            var wktReader = new WKTReader();
            Geometry geometry = wktReader.Read(wkt);

            var wktWriter = new WKTWriter();
            wkt = wktWriter.Write(polygon);

            var p = (Polygon)geometry;

            var points = new List<(double, double)>();
            foreach (var coordinate in p.Coordinates)
            {
                points.Add((coordinate.X, coordinate.Y));
            }

            p.SRID = SRID;
            _myDbContext.User.Add(new User
            {
                PlacePoly = p
            });
            _myDbContext.SaveChanges();
            
            return new ResultDto
            {
                IsContains = p.Contains(new Point(new Coordinate(120, 120))),
                PolygonAsText = p.AsText(),
                PolygonAsBinary = p.AsBinary(),
                Coordinates = p.Coordinates,
                LineString = p.ExteriorRing,
                Shell = p.Shell,
                UserData = p.UserData,
                polygonToString = p.ToString(),
                WKT = wkt,
                Points = points
            };
        }
    }


    public class ResultDto
    {
        public bool IsContains { get; set; }

        public string PolygonAsText { get; set; }

        public byte[] PolygonAsBinary { get; set; }

        public Coordinate[] Coordinates { get; set; }

        public object UserData { get; set; }

        public string polygonToString { get; set; }

        public string WKT { get; set; }

        public List<(double, double)> Points { get; set; }

        public LineString LineString { get; set; }

        public LinearRing Shell { get; set; }
    }
}
