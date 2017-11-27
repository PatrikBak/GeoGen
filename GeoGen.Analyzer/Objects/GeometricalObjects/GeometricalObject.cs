using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects.GeometricalObjects
{
    internal abstract class GeometricalObject
    {
        public int Id { get; }

        public ConfigurationObject ConfigurationObject { get; set; }

        protected GeometricalObject(ConfigurationObject configurationObject, int id)
        {
            ConfigurationObject = configurationObject ?? throw new ArgumentNullException(nameof(configurationObject));
            Id = id;
        }

        protected GeometricalObject(int id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return ((GeometricalObject) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}