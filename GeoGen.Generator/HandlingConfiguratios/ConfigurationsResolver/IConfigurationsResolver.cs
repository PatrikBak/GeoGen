namespace GeoGen.Generator
{
    internal interface IConfigurationsResolver
    {
        bool ResolveNewOutput(ConstructorOutput output);

        void ResolveInitialConfiguration(ConfigurationWrapper configuration);
    }
}