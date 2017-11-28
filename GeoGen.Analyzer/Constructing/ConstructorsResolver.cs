using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer.Constructing
{
    /// <summary>
    /// A default implementation of <see cref="IConstructorsResolver"/>.
    /// </summary>
    internal sealed class ConstructorsResolver : IConstructorsResolver
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping types of predefined constructions to their 
        /// predefined constructors.
        /// </summary>
        private readonly Dictionary<Type, IPredefinedConstructor> _constructors; 

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new constructors resolver with provided predefined
        /// constructors.
        /// </summary>
        /// <param name="constructors">The predefined constructors array.</param>
        public ConstructorsResolver(IPredefinedConstructor[] constructors)
        {
            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            _constructors = new Dictionary<Type, IPredefinedConstructor>();

            foreach (var constructor in constructors)
            {
                if (constructor == null)
                    throw new ArgumentException("Constructors can't contain null objects");

                var type = constructor.PredefinedConstructionType;

                if (_constructors.ContainsKey(type))
                    throw new ArgumentException("Duplicate constructors");

                _constructors.Add(type, constructor);
            }
        }

        #endregion

        #region IConstructorsResolver implementation

        /// <summary>
        /// Find the corresponding <see cref="IObjectsConstructor"/> for 
        /// a given <see cref="Construction"/>.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The constructor.</returns>
        public IObjectsConstructor Resolve(Construction construction)
        {
            if (construction == null)
                throw new ArgumentNullException(nameof(construction));

            if (construction is PredefinedConstruction predefinedConstruction)
            {
                try
                {
                    return _constructors[predefinedConstruction.GetType()];
                }
                catch (KeyNotFoundException)
                {
                    throw new AnalyzerException("Unresolvable predefined constructor.");
                }
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}