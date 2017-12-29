using System.Collections.Generic;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer
{
    internal interface ITheoremVerifier
    {
        TheoremType TheoremType { get; }

        IEnumerable<VerifierOutput> GetOutput(VerifierInput verifierInput);
    }
}