using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IContainer{T}"/>, where 'T' is <see cref="ConfigurationObject"/>.
    /// This container is able to recognize equal arguments consisting of correctly created objects
    /// (i.e. fulfilling the invariant that two objects are formally equal if and only if they have 
    /// distinct references). This class makes use of <see cref="StringBasedContainer{T}"/> together with
    /// <see cref="DefaultArgumentsToStringConverter"/>.
    /// </summary>
    public class ArgumentsContainer : StringBasedContainer<Arguments>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsContainer"/> class.
        /// </summary>
        /// <param name="converter">The converter of arguments to a string used by the string based container.</param>
        public ArgumentsContainer(DefaultArgumentsToStringConverter converter)
            : base(converter)
        {
        }
    }
}
