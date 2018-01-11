using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a container of all used <see cref="IObjectIdResolver"/>s.
    /// These resolvers should represent all possible permutations of the ids
    /// of <see cref="LooseConfigurationObject"/>s. It implements the
    /// <see cref="IEnumerable{T}"/> interface, where T is <see cref="IObjectIdResolver"/>.
    /// </summary>
    internal interface IObjectIdResolversContainer : IEnumerable<IObjectIdResolver>
    {
    }
}