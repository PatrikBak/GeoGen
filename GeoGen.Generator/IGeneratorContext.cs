namespace GeoGen.Generator
{
    internal interface IGeneratorContext
    {
        IConfigurationContainer ConfigurationContainer { get; }

        IConfigurationsHandler ConfigurationsHandler { get; }

        IConfigurationConstructer ConfigurationConstructer { get; }
    }
}
