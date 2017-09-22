using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString
{
    /// <summary>
    /// A base class that implements <see cref="IObjectToStringProvider"/>. The main
    /// purpose of this class is to override the equals and hash code methods
    /// so that the providers can be efficiently used as keys for a dictionary.
    /// The providers are compared by the ids of the <see cref="IObjectIdResolver"/>s
    /// that they use.
    /// </summary>
    internal abstract class ObjectToStringProviderBase : IObjectToStringProvider
    {
        #region IObjectToStringProvider properties

        /// <summary>
        /// Gets the object to string resolver that is used by this provider.
        /// </summary>
        public IObjectIdResolver Resolver { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new object to string provider that uses a given
        /// object id resolver.
        /// </summary>
        /// <param name="resolver">The object id resolver.</param>
        protected ObjectToStringProviderBase(IObjectIdResolver resolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IObjectToStringProvider methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        public abstract string ConvertToString(ConfigurationObject configurationObject);

        #endregion

        #region Equals and HashCode

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            var casted = (DefaultObjectToStringProvider) obj;

            return Resolver.Id.Equals(casted.Resolver.Id);
        }

        public override int GetHashCode()
        {
            return Resolver.Id;
        }

        #endregion
    }
}