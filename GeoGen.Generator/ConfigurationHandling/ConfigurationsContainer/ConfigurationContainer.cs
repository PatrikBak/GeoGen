using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer
{
    internal class ConfigurationContainer : IConfigurationContainer
    {
        private readonly List<ConfigurationWrapper> _configurations = new List<ConfigurationWrapper>();

        private readonly IArgumentsContainerFactory _argumentsContainerFactory;

        public void Initialize(Configuration initialConfiguration)
        {
            _configurations.Clear();
            
            _configurations.Add(new ConfigurationWrapper(){});
        }

        public void AddLayer(List<ConstructorOutput> newLayerOutput)
        {
        }

        #region IEnumerable methods

        public IEnumerator<ConfigurationWrapper> GetEnumerator()
        {
            return _configurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}