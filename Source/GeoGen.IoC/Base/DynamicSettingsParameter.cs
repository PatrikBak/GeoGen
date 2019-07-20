using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;
using System;

namespace GeoGen.IoC
{
    /// <summary>
    /// Represents a <see cref="IParameter"/> that holds an object with dynamic settings
    /// that will be passed to the corresponding dependency. This can only work if there is exactly
    /// one dependency expecting this type of settings.
    /// </summary>
    public class DynamicSettingsParameter : IParameter
    {
        #region IParameter properties

        /// <summary>
        /// Makes sure this parameters are stored in the context during the whole request.
        /// </summary>
        public bool ShouldInherit => true;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the value of the settings.
        /// </summary>
        public object Value { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSettingsParameter"/> class.
        /// </summary>
        /// <param name="value">The value of the settings.</param>
        public DynamicSettingsParameter(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region Unnecessary inherited methods and properties

        public string Name => throw new NotImplementedException();

        public bool Equals(IParameter other) => throw new NotImplementedException();

        public object GetValue(IContext context, ITarget target) => throw new NotImplementedException();

        #endregion
    }
}