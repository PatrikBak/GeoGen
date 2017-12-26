namespace GeoGen.Generator
{
    internal interface IConfigurationsResolver
    {
        bool ResolveNewOutput(ConstructorOutput output);

        bool ResolveMappedOutput(ConfigurationWrapper configuration);

        void ResolveInitialConfiguration(ConfigurationWrapper configuration);
    }
}