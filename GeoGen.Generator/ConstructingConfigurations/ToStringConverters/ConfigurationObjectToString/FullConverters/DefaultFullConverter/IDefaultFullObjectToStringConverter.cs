using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IFullConfigurationToStringConverter"/> that is meant to be
    /// a converter for <see cref="ConfigurationObject"/>s that don't have their ids set yet.
    /// This service, however, assumes, that all their underlying objects have their ids and 
    /// that they also have their string versions cached. This caching must be done manually 
    /// using the cache method, since the id is not known during the to string conversion process.
    /// </summary>
    internal interface IDefaultFullObjectToStringConverter : IFullObjectToStringConverter
    {
        /// <summary>
        /// Caches a string version associated with the object of a given id. We call this manually
        /// because the id is not supposed to be set before the first call the convert method.
        /// </summary>
        /// <param name="configurationObjectId">The configuration object id.</param>
        /// <param name="stringVersion">The string version.</param>
        void CacheObject(int configurationObjectId, string stringVersion);
    }
}