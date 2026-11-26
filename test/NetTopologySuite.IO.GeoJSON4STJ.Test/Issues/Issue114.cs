using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using NetTopologySuite.IO.GeoJSON4STJ.Test.Converters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTopologySuite.IO.GeoJSON4STJ.Test.Issues
{
    internal class Issue114 : SandDTest<Geometry>
    {
        [Test, GeoJsonIssueNumber(114)]
        public void TestSerializeDoesNotChangeOrientationOfInputSequence()
        {
            const string wkt = "POLYGON((0 0, 0 10, 10 10, 10 0, 0 0))";
            var g = (Polygon)new WKTReader().Read(wkt);

            var seq1 = g.ExteriorRing.CoordinateSequence.Copy();
            var opt = new System.Text.Json.JsonSerializerOptions();
            opt.Converters.Add(new GeoJsonConverterFactory(
                NtsGeometryServices.Instance.CreateGeometryFactory(4326),
                false, GeoJsonConverterFactory.DefaultIdPropertyName,
                RingOrientationOption.EnforceRfc9746));

            using var ms = new MemoryStream();
            Serialize(ms, g, opt);
            var seq2 = g.ExteriorRing.CoordinateSequence.Copy();

            Assert.That(seq1.ToCoordinateArray(), Is.EqualTo(seq2.ToCoordinateArray()));
        }
    }
}
