using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref=IObjectIntroducer/> that applies <see cref="ObjectIntroductionRule"/>s
    /// that are received in an <see cref="ObjectIntroducerData"/> objects.
    /// </summary>
    public class ObjectIntroducer : IObjectIntroducer
    {
        #region Private fields

        /// <summary>
        /// The data with the rules for the introducer.
        /// </summary>
        private readonly ObjectIntroducerData _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="IObjectIntroducer"/> class.
        /// </summary>
        /// <param name="data">The data with the rules for the introducer.</param>
        public ObjectIntroducer(ObjectIntroducerData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        #endregion

        #region IObjectIntroducer implementation

        /// <inheritdoc/>
        public IEnumerable<ConstructedConfigurationObject> IntroduceObjects(IReadOnlyList<ConstructedConfigurationObject> availableObjects)
            // We will try every object
            => availableObjects.SelectMany(availableObject =>
            {
                // Find the rule for this one
                var rule = _data.Rules.GetValueOrDefault(availableObject.Construction);

                // If there is no rule for this construction, then we can't do much
                if (rule == null)
                    return Enumerable.Empty<ConstructedConfigurationObject>();

                // Otherwise we remap the argument objects one-to-one. 
                var mapping = rule.ExistingObject.PassedArguments.FlattenedList
                    // First we zip them
                    .Zip(availableObject.PassedArguments.FlattenedList)
                    // Make a dictionary for comfort
                    .ToDictionary(pair => pair.First, pair => pair.Second);

                // The template object might be in the arguments of the introduces objects 
                // too, so we better add it to the mapping
                mapping.Add(rule.ExistingObject, availableObject);

                // Now we can comfortably remap the introduced objects
                return rule.NewObjects
                    // For a given one we copy the construction
                    .Select(newObject => new ConstructedConfigurationObject(newObject.Construction,
                        // And remap the arguments according to the constructed mapping
                        newObject.PassedArguments.FlattenedList.Select(argument => mapping[argument]).ToArray()));
            });

        #endregion
    }
}