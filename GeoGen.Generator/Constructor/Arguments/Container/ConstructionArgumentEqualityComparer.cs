using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.Constructor.Arguments.Container
{
    public class ConstructionArgumentEqualityComparer : IEqualityComparer<ConstructionArgument>
    {
        private readonly IEqualityComparer<ConfigurationObject> _configurationObjectsComparer;

        public ConstructionArgumentEqualityComparer(IEqualityComparer<ConfigurationObject> configurationObjectsComparer)
        {
            _configurationObjectsComparer = configurationObjectsComparer;
        }

        public bool Equals(ConstructionArgument x, ConstructionArgument y)
        {
            if (x.GetType() != y.GetType())
                return false;

            if (x is ObjectConstructionArgument xObj && y is ObjectConstructionArgument yObj)
            {
                return _configurationObjectsComparer.Equals(xObj.PassedObject, yObj.PassedObject);
            }

            var xSet = x as SetConstructionArgument ?? throw new NullReferenceException("Unhandled case.");
            var ySet = y as SetConstructionArgument ?? throw new NullReferenceException("Unhandled case.");

            // building sets would cause recursive calls of this function
            var xSetElements = new HashSet<ConstructionArgument>(xSet.PassedArguments, this);
            var ySetElements = new HashSet<ConstructionArgument>(ySet.PassedArguments, this);

            return xSetElements.SetEquals(ySetElements);
        }

        public int GetHashCode(ConstructionArgument obj)
        {
            if (obj is ObjectConstructionArgument objectArgument)
            {
                return _configurationObjectsComparer.GetHashCode(objectArgument.PassedObject);
            }

            var setArgument = obj as SetConstructionArgument ?? throw new NullReferenceException("Unhandled argument type");
            var passedArguments = setArgument.PassedArguments;

            // this would case resursive calls of this function
            return HashCodeUtilities.GetOrderIndependentHashCode(passedArguments, GetHashCode);
        }
    }
}