using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects.GeometricalObjects
{
    public abstract class GeometricalObject : IEquatable<GeometricalObject>
    {
        public int Id { get; }

        public ConfigurationObject ConfigurationObject { get; set; }

        protected GeometricalObject(ConfigurationObject configurationObject, int id)
        {
            ConfigurationObject = configurationObject;
            Id = id;
        }

        protected GeometricalObject(int id)
        {
            Id = id;
        }

        public bool Equals(GeometricalObject other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((GeometricalObject) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}