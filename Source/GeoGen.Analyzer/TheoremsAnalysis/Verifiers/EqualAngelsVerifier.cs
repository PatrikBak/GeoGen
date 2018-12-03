using GeoGen.AnalyticalGeometry;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.EqualAngles"/>.
    /// </summary>
    internal class EqualAnglesVerifier : TheoremVerifierBase
    {
        #region Private fields

        /// <summary>
        /// The generator of subsets of given size.
        /// </summary>
        private readonly ISubsetsProvider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The subsets generator.</param>
        public EqualAnglesVerifier(ISubsetsProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region ITheoremVerifier implementation

        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container)
        {
            // Find new lines. 
            var newLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // Find old lines. 
            var oldLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // Local function to enumerate all pairs of lines containing at least one new line
            IEnumerable<(LineObject line1, LineObject line2)> PairsOfLines()
            {
                // First combine the new lines with themselves
                for (var i = 0; i < newLines.Count; i++)
                {
                    for (var j = i + 1; j < newLines.Count; j++)
                    {
                        yield return (newLines[i], newLines[j]);
                    }
                }

                // Now combine new lines with old ones
                foreach (var newLine in newLines)
                {
                    foreach (var oldLine in oldLines)
                    {
                        yield return (newLine, oldLine);
                    }
                }
            }

            // Local function that calculates the angel between two lines
            // with respect to a passed container
            RoundedDouble AngleBetween((LineObject line1, LineObject line2) lines, IObjectsContainer objectsContainer)
            {
                // Pull analytial versions of these lines
                var analyticalLine1 = container.GetAnalyticalObject<Line>(lines.line1, objectsContainer);
                var analyticalLine2 = container.GetAnalyticalObject<Line>(lines.line2, objectsContainer);

                // Return their distance
                return (RoundedDouble)analyticalLine1.AngleBetween(analyticalLine2);
            }

            // Prepare a dictionary mapping angles to lists of pairs of lines forming an angel of this size
            // This is enumerator with respect to some of the containers. 
            var anglesMap = new Dictionary<RoundedDouble, List<(LineObject line1, LineObject line2)>>();

            // Pull the first container acorrding to which we're going to fill the map
            var firstContainer = container.Manager.First();

            // Enumerate all valid pairs of lines using our local function
            foreach (var lineSegment in PairsOfLines())
            {
                // Find the angle between them in the first container
                var angle = AngleBetween(lineSegment, firstContainer);

                // Find the right list of pairs of lines with this angle
                var listOfSegments = anglesMap.GetOrAdd(angle, () => new List<(LineObject line1, LineObject line2)>());

                // Add the pair of lines to the list
                listOfSegments.Add(lineSegment);
            }

            // Now we're interested only in those lists that have at least 2 elements (i.e. 
            // there are at least two pairs of lines forming this angle). We will take into
            // account onle those ones with respect to the other containers
            var pairsOfLinesLists = anglesMap.Values.Where(list => list.Count >= 2);

            // For each of them
            foreach (var pairOfLines in pairsOfLinesLists)
            {
                // We'll take all pairs of these pairs
                foreach (var pairOfPairOfLines in _provider.GetSubsets(pairOfLines, 2))
                {
                    // Enumerate the pair
                    var pair = pairOfPairOfLines.ToArray();

                    // Construct the verifier function 
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // If the container is the first one, we're sure the objects are fine
                        if (ReferenceEquals(firstContainer, objectsContainer))
                            return true;

                        // Otherwise we check if their angles are equal in this container
                        return AngleBetween(pair[0], objectsContainer) == AngleBetween(pair[1], objectsContainer);
                    }

                    // Construct the output
                    yield return new PotentialTheorem
                    {
                        TheoremType = Type,
                        InvolvedObjects = new[] { pair[0].line1, pair[0].line2, pair[1].line1, pair[1].line2 },
                        VerifierFunction = Verify
                    };
                }
            }
        }
        
        #endregion
    }
}