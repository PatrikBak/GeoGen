using GeoGen.Analyzer;
using System;
using System.IO;

namespace GeoGen.ConsoleTest
{
    public class ConsoleInconsistenciesTracker : IInconsistentContainersTracer
    {
        private int all;

        private int one;

        private readonly StreamWriter _writer = new StreamWriter("inconsistencies.txt");

        public void TraceReachingMaximalNumberOfAttemptsToReconstructAllContainers(IObjectsContainersManager manager)
        {
            _writer.WriteLine($"{++all}. We've reached the maximal for all containers.");
        }

        public void TraceReachingMaximalNumberOfAttemptsToReconstructOneContainer(IObjectsContainersManager manager, IObjectsContainer container)
        {
            _writer.WriteLine($"{++one}. We've reached the maximal for one container.");
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