using System;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer
{
    internal class AnalyzerInitializer : IAnalyzerInitializer
    {
        private readonly IGeometryRegistrar _registrar;

        public AnalyzerInitializer(IGeometryRegistrar registrar)
        {
            // TODO: Test
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
        }

        public void Initialize(Configuration initialConfiguration)
        {
            _registrar.Initialize(initialConfiguration);
        }
    }
}