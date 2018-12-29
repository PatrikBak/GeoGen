using System;
using GeoGen.Analyzer;

namespace GeoGen.ConsoleTest
{
    public class ConsoleInconsistenciesTracker : IInconsistentContainersTracer
    {
        private int all;

        private int one;

        public void TraceReachingMaximalNumberOfAttemptsToReconstructAllContainers(IObjectsContainersManager manager)
        {
            all++;
            Console.WriteLine($"{all}. We've reached the maximal for all containers.");
        }

        public void TraceReachingMaximalNumberOfAttemptsToReconstructOneContainer(IObjectsContainersManager manager, IObjectsContainer container)
        {
            one++;
            Console.WriteLine($"{one}. We've reached the maximal for one container.");
        }

        public void TraceResolvedInconsistency(IObjectsContainersManager manager, int attemptsToReconstruct)
        {
            Console.WriteLine($"Resolved inconsistency with {attemptsToReconstruct} reconstructions.");
        }

        public void TraceUnsuccessfulAtemptsToReconstructOneContainer(IObjectsContainersManager manager, IObjectsContainer container, int attemptsToReconstruct)
        {
            Console.WriteLine($"{attemptsToReconstruct} unsuccessful attempts to reconstruct.");
        }
    }
}