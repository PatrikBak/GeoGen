using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;

namespace GeoGen.Analyzer.Constructing.PredefinedConstructors
{
    internal class InteresectionConstructor : IPredefinedConstructor
    {
        public Type PredefinedConstructionType { get; } = typeof(Intersection);

        public List<GeometricalObject> Apply(IReadOnlyList<ConstructionArgument> arguments, IObjectsContainer container)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            try
            {
                var setArgument1 = (SetConstructionArgument) arguments[0];
                var setArgument2 = (SetConstructionArgument) arguments[1];

                var passedPoints1 = setArgument1.PassedArguments.ToList();
                var passedPoints2 = setArgument2.PassedArguments.ToList();

                var obj1 = ((ObjectConstructionArgument) passedPoints1[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints1[1]).PassedObject;
                var obj3 = ((ObjectConstructionArgument) passedPoints2[0]).PassedObject;
                var obj4 = ((ObjectConstructionArgument) passedPoints2[1]).PassedObject;

                var point1 = (Point) container[obj1.Id ?? throw new Exception()];
                var point2 = (Point) container[obj2.Id ?? throw new Exception()];
                var point3 = (Point) container[obj3.Id ?? throw new Exception()];
                var point4 = (Point) container[obj4.Id ?? throw new Exception()];

                var result = AnalyticalHelpers.IntersectionOfLines(point1, point2, point3, point4);

                return new List<GeometricalObject> {result};
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}