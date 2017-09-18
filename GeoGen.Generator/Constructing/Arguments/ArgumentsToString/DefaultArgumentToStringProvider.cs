using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    internal class DefaultArgumentToStringProvider : ArgumentToStringProviderBase
    {
        public DefaultArgumentToStringProvider(DefaultObjectToStringProvider objectToString, string intersetSeparator)
            : base(objectToString, intersetSeparator)
        {
        }

        public DefaultArgumentToStringProvider(DefaultObjectToStringProvider objectToString)
            : base(objectToString)
        {
        }

        protected override string ResolveCachedValue(ConstructionArgument constructionArgument)
        {
            var id = constructionArgument.Id;

            // If the argument doesn't have an id, we can't have it cached
            if (!id.HasValue)
            {
                return string.Empty;
            }

            try
            {
                // We assume that we must have cached all objects (if not, this will cause 
                // KeyNotFound exception, so we'll hit the catch block)
                return Cache[id.Value];
            }
            catch (KeyNotFoundException)
            {
                throw new GeneratorException("The object with this id hasn't been cached.");
            }
        }

        protected override void HandleResult(ConstructionArgument constructionArgument, string result)
        {
            // We can't do anything useful here. The id of the argument is not supposed be set yet.
        }

        /// <summary>
        /// Manually caches the given argument. This should be called after
        /// converting an argument that didn't have the id during the conversion.
        /// </summary>
        /// <param name="argumentId">The argument id.</param>
        /// <param name="stringVersion">The strin version.</param>
        public void CacheArgument(int argumentId, string stringVersion)
        {
            try
            {
                Cache.Add(argumentId, stringVersion);
            }
            catch (ArgumentException)
            {
                throw new GeneratorException("The argument with this id has been already cached.");
            }
        }
    }
}