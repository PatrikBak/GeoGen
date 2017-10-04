using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems.TheoremVerifiers
{
    internal class ConcurrencyVerifier : ITheoremVerifier
    {
        public List<Dictionary<ConfigurationObjectType, int>> Signatures { get; }

        public ConcurrencyVerifier()
        {
            Signatures = new List<Dictionary<ConfigurationObjectType, int>>
            {
                new Dictionary<ConfigurationObjectType, int>
                {
                    {ConfigurationObjectType.Point, 6}
                },
                new Dictionary<ConfigurationObjectType, int>
                {
                    {ConfigurationObjectType.Point, 4},
                    {ConfigurationObjectType.Line, 1}
                },
                new Dictionary<ConfigurationObjectType, int>
                {
                    {ConfigurationObjectType.Point, 2},
                    {ConfigurationObjectType.Line, 2}
                },
                new Dictionary<ConfigurationObjectType, int>
                {
                    {ConfigurationObjectType.Line, 3}
                }
            };
        }

        public bool Verify(ConfigurationObjectsMap objects, int signatureIndex, IObjectsContainer container)
        {
            if (signatureIndex != 0)
                return false;

            var points = objects[ConfigurationObjectType.Point]
                    .Select(container.Get<Point>)
                    .ToList();

            var intersetion = AnalyticalHelpers.IntersectionOfLines(points.Take(4).ToList());
            var otherPointsWithIntersection = points.Skip(4).ConcatItem(intersetion).ToList();

            return AnalyticalHelpers.ArePointsCollinear(otherPointsWithIntersection);
        }

        public Theorem ConstructTheorem(ConfigurationObjectsMap input, int signatureIndex)
        {
            if (signatureIndex != 0)
                return null;

            return null;
        }
    }
}