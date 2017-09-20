using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString
{
    internal abstract class ObjectToStringProviderBase : IObjectToStringProvider
    {
        public IObjectIdResolver Resolver { get; }

        protected ObjectToStringProviderBase(IObjectIdResolver resolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public abstract string ConvertToString(ConfigurationObject configurationObject);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            var casted = (DefaultObjectToStringProvider) obj;

            return Resolver.Equals(casted.Resolver);
        }

        public override int GetHashCode()
        {
            return Resolver.Id;
        }
    }
}