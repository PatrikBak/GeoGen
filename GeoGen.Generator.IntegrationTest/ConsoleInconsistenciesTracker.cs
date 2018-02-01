using System;
using GeoGen.Analyzer;

namespace GeoGen.Generator.IntegrationTest
{
    internal class ConsoleInconsistenciesTracker : IInconsistenciesTracker
    {
        public int AttemptsToReconstruct { get; private set; }

        public int Inconsistencies { get; private set; }

        public void OnUnsuccessfulAttemptToReconstructContainer()
        {
            AttemptsToReconstruct++;
        }

        public void MarkInconsistency()
        {
            Inconsistencies++;
        }
    }
}