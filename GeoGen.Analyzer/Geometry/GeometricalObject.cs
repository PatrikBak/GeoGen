using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Geometry
{
    /// <summary>
    /// A base class for all geometrical objects represented in a real 
    /// coordinate system. 
    /// </summary>
    internal abstract class GeometricalObject
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration object associated with this geometrical object.
        /// </summary>
        public ConfigurationObject ConfigurationObject { get; set; }

        #endregion

        //#region Abstract properties

        ///// <summary>
        ///// Checks if a given other is equal to this one.
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //public abstract bool Equals(GeometricalObject other);

        //public abstract override int GetHashCode();

        //#endregion

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj))
        //        return false;

        //    if (ReferenceEquals(this, obj))
        //        return true;

        //    if (obj.GetType() != GetType())
        //        return false;

        //    return Equals((GeometricalObject) obj);
        //}
    }
}