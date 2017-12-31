namespace GeoGen.Generator
{
    internal interface IDefaultFullObjectToStringConverter : IObjectToStringConverter
    {
        /// <summary>
        /// Caches a string version associated with the object of a given id. We call this manually
        /// because the id is not supposed to be set when we first call the convert method.
        /// </summary>
        /// <param name="configurationObjectId">The configuration object id.</param>
        /// <param name="stringVersion">The string version.</param>
        void CacheObject(int configurationObjectId, string stringVersion);
    }
}