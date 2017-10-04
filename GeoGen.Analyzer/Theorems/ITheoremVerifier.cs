using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremVerifier
    {
        List<Dictionary<ConfigurationObjectType, int>> Signatures { get; }

        bool Verify(ConfigurationObjectsMap objects, int signatureIndex, IObjectsContainer container);

        Theorem ConstructTheorem(ConfigurationObjectsMap input, int signatureIndex);
    }
}