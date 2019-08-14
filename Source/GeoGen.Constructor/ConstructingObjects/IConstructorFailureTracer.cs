using System;
using System.Collections.Generic;
using System.Text;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a tracer of unexpected <see cref="AnalyticException"/> caused
    /// by <see cref="IObjectsConstructor"/>s.
    /// </summary>
    public interface IConstructorFailureTracer
    {
        void TraceUnexpectedConstructionFailure(ConstructedConfigurationObject configurationObject, IAnalyticObject[] analyticObjects, string message);
    }
}
