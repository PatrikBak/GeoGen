using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object that can be identified once, not necessarily during its construction, and then the its id cannot be changed.
    /// It overrides the hash code and equal methods so the id is used for comparison if there is any.
    /// </summary>
    public abstract class IdentifiedObject //: IEquatable<IdentifiedObject>
    {
        #region Private fields

        /// <summary>
        /// The backing field for the <see cref="Id"/> property.
        /// </summary>
        private int? _id;

        #endregion
               
        #region Public properties

        /// <summary>
        /// Gets or sets the id of this configuration object. The id should be set only once.
        /// Setting it more than once, or accesing it when it's not set, causes a <see cref="GeoGenException"/>. 
        /// </summary>
        public int Id
        {
            get => _id ?? throw new InvalidOperationException("The id hasn't been set yet.");
            set => _id = !_id.HasValue ? value : throw new InvalidOperationException("The id of this object has been already set and cannot be changed.");
        }

        /// <summary>
        /// Indicates if this object is identified, i.e. if the <see cref="Id"/> property has been set.
        /// </summary>
        public bool HasId => _id.HasValue;

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Finds out if the passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The other object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Do the null and type check and then call the other Equals method
            return otherObject != null && otherObject is IdentifiedObject identifiedObject && Equals(identifiedObject);
        }

        /// <summary>
        /// Gets the hash code if this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => Id;

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Finds out if the passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The other object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool Equals(IdentifiedObject otherObject) => Id == otherObject.Id;

        #endregion
    }
}
