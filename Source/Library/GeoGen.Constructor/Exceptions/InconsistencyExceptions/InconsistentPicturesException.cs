namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an exception that is thrown when there is an inconsistency between 
    /// <see cref="Picture"/>s. Two pictures are inconsistent for example if there is 
    /// an object that is constructible in one of them, and not in the other. The other 
    /// cases of inconsistency might be: two objects are not duplicates in all pictures, 
    /// three points are not collinear in all pictures, etc. Inconsistencies are caused 
    /// either due to our limited precision model (which happens rarely), or because of 
    /// constructions such as MidpointOfArc, that yield equal objects in half cases.
    /// </summary>
    public class InconsistentPicturesException : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentPicturesException"/> class.
        /// </summary>
        public InconsistentPicturesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentPicturesException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public InconsistentPicturesException(string message)
                : base(message)
        {
        }
    }
}