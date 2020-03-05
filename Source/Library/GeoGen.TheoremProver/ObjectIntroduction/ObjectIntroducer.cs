using GeoGen.Core;
using System.Collections.Generic;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref=IObjectIntroducer/>.
    /// <para>
    /// TODO: Replace this temporary implementation.
    /// </para>
    /// </summary>
    public class ObjectIntroducer : IObjectIntroducer
    {
        /// <inheritdoc/>
        public IEnumerable<ConstructedConfigurationObject> IntroduceObjects(IReadOnlyList<ConstructedConfigurationObject> availableObjects)
        {
            foreach (var availableObject in availableObjects)
            {
                if (availableObject.Construction.Name.Equals(nameof(ReflectionInLine)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[1];

                    yield return new ConstructedConfigurationObject(Midpoint, availableObject, A);
                }

                else if (availableObject.Construction.Name.Equals(nameof(ReflectionInLineFromPoints)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];

                    yield return new ConstructedConfigurationObject(Midpoint, availableObject, A);
                }

                else if (availableObject.Construction.Name.Equals(nameof(PerpendicularBisector)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];

                    yield return new ConstructedConfigurationObject(Midpoint, A, B);
                }

                else if (availableObject.Construction.Name.Equals(nameof(ParallelogramPoint)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Midpoint, B, C);
                    yield return new ConstructedConfigurationObject(Median, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Incenter)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];
                    var I = availableObject;

                    yield return new ConstructedConfigurationObject(Incircle, A, B, C);
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, B, A, C);
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, C, A, B);
                    yield return new ConstructedConfigurationObject(Circumcenter, B, I, C);
                    yield return new ConstructedConfigurationObject(Circumcenter, C, I, A);
                    yield return new ConstructedConfigurationObject(Circumcenter, A, I, B);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Centroid)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Median, A, B, C);
                    yield return new ConstructedConfigurationObject(Median, B, A, C);
                    yield return new ConstructedConfigurationObject(Median, C, A, B);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Incircle)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(NinePointCircle, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Excenter)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];
                    var E = availableObject;

                    yield return new ConstructedConfigurationObject(ExternalAngleBisector, B, A, C);
                    yield return new ConstructedConfigurationObject(ExternalAngleBisector, C, A, B);
                    yield return new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
                    yield return new ConstructedConfigurationObject(Circumcenter, B, E, C);
                    yield return new ConstructedConfigurationObject(Circumcenter, C, E, A);
                    yield return new ConstructedConfigurationObject(Circumcenter, A, E, B);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Excircle)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(NinePointCircle, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Midline)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Midpoint, A, B);
                    yield return new ConstructedConfigurationObject(Midpoint, A, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(Median)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(ParallelogramPoint, A, B, C);
                    yield return new ConstructedConfigurationObject(Centroid, A, B, C);
                    yield return new ConstructedConfigurationObject(Midpoint, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(TangentLine)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Circumcenter, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(LineThroughCircumcenter)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Circumcenter, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(OppositePointOnCircumcircle)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(Circumcenter, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(MidpointOfArc)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(ExternalAngleBisector, A, B, C);
                }

                else if (availableObject.Construction.Name.Equals(nameof(MidpointOfOppositeArc)))
                {
                    var A = availableObject.PassedArguments.FlattenedList[0];
                    var B = availableObject.PassedArguments.FlattenedList[1];
                    var C = availableObject.PassedArguments.FlattenedList[2];

                    yield return new ConstructedConfigurationObject(InternalAngleBisector, A, B, C);
                }
            }
        }
    }
}