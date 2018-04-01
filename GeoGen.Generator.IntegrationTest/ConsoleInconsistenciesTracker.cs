using GeoGen.Analyzer;
using System;

namespace GeoGen.Generator.IntegrationTest
{
    internal class ConsoleInconsistenciesTracker : IInconsistenciesTracker
    {
        public int AttemptsToReconstruct { get; private set; }

        public int Inconsistencies { get; private set; }

        public void OnUnsuccessfulAttemptToReconstructContainer()
        {
            AttemptsToReconstruct++;

            Console.WriteLine(AttemptsToReconstruct);
        }

        public void MarkInconsistency()
        {
            Inconsistencies++;
        }
    }
}