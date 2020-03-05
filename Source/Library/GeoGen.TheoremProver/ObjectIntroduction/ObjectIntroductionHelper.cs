using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a helper that takes care of object introduction and removal. It uses an <see cref="IObjectIntroducer"/>
    /// to handle actual deciding which objects should be introduced. It then handles all the needed communication with
    /// a <see cref="NormalizationHelper"/> that needs to know about these objects. 
    /// <para>
    /// The object introduction is done in the following way:
    /// <list type="bullet">
    /// <item>Only objects from the original configuration or objects equal to them are used as available objects
    /// that are passed to <see cref="IObjectIntroducer.IntroduceObjects(IReadOnlyList{ConstructedConfigurationObject})"/>.
    /// This step is important to prevent perhaps even infinite explosion of object introduction.</item>
    /// <item>Only objects that are not among <see cref="NormalizationHelper.AllObjects"/> are introduced.</item>
    /// <item>Only objects that are not among <see cref="NormalizationHelper.AllRemovedObjects"/> are introduced.
    /// This step ensures that we will not get into a loop of introducing and removing the same object.</item>
    /// <item>Every time we introduce a point, the previously introduced points are removed. The reason why we do it is 
    /// that we do not want to have too many introduced points, because it could uncontrollably slow down the processing. 
    /// The reason why we remove only points is that introduced lines and circles are usually already there implicitly 
    /// (by points) and generally their processing is much less time consuming than that of points.</item>
    /// <item>If there is an option to introduce a line or circle, then it is preferred over introducing points.</item>
    /// </list>
    /// </para>
    /// </summary>
    public class ObjectIntroductionHelper
    {
        #region Dependencies

        /// <summary>
        /// The introducer of new objects to which deciding based on available objects is delegated.
        /// </summary>
        private readonly IObjectIntroducer _introducer;

        /// <summary>
        /// The normalization helper that needs to be know about all objects and therefore needs to be communicated with.
        /// </summary>
        private readonly NormalizationHelper _helper;

        #endregion

        #region Private fields

        /// <summary>
        /// The object that has been introduced via the last call of the <see cref="IntroduceObjects"/> method.
        /// </summary>
        private ConstructedConfigurationObject _lastIntroducedObject;

        /// <summary>
        /// All the objects that have been introduced via the last call of the <see cref="IntroduceObjects"/> method.
        /// </summary>
        private readonly HashSet<ConstructedConfigurationObject> _allIntroducedObjects = new HashSet<ConstructedConfigurationObject>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionHelper"/> class.
        /// </summary>
        /// <param name="introducer">The introducer of new objects to which deciding based on available objects is delegated.</param>
        /// <param name="helper">The normalization helper that needs to be know about all objects and therefore needs to be communicated with.</param>
        public ObjectIntroductionHelper(IObjectIntroducer introducer, NormalizationHelper helper)
        {
            _introducer = introducer ?? throw new ArgumentNullException(nameof(introducer));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Introduces a new object and handles all the needed communicated with the normalization helper.
        /// </summary>
        /// <returns>
        /// The tuple of objects that got removed and the object that got introduced. The object list will never 
        /// be null, but might be empty. Also, the number of removed objects might be bigger than the number of 
        /// previously introduced ones, because the normalization helper might have found equal ones in the meantime.
        /// If there has been no introduces, then the new object will be null.
        /// </returns>
        public (IReadOnlyList<ConstructedConfigurationObject> removedObjects, ConstructedConfigurationObject newObject) IntroduceObject()
        {
            #region Handle object removal

            // Prepare the removed objects
            var removedObjects = new List<ConstructedConfigurationObject>();

            // If the last introduced object was a point
            if (_lastIntroducedObject != null && _lastIntroducedObject.ObjectType == ConfigurationObjectType.Point)
            {
                // Let the normalization helper remove it, while returning everything what's been removed
                _helper.RemoveIntroducedObject(_lastIntroducedObject, out var removedObjectsLocal);

                // Mark the removed objects
                removedObjects.Add(removedObjectsLocal);
            }

            #endregion

            #region Handle object introduction

            // Find the object to introduce by calling the introducer
            var newObject = _introducer
                // With the available objects equal to those 
                .IntroduceObjects(_helper.AllObjects
                    // That are in the original configuration (otherwise it would be explosive)
                    .Where(_helper.IsInOriginalConfiguration)
                    // Enumerated
                    .ToArray())
                // The introducer offers us some options that we need to normalize by copying the construction
                .Select(newObject => new ConstructedConfigurationObject(newObject.Construction,
                        // And normalizing each of the argument objects
                        newObject.PassedArguments.FlattenedList.Select(_helper.GetNormalVersionOfObjectOrNull).ToArray()))
                // When we have options normalize, we need to filter those objects that have been introduced already
                .Where(newObject => !_allIntroducedObjects.Contains(newObject)
                        // And aren't already known to the helper
                        && !_helper.AllObjects.Contains(newObject)
                        // And haven't been ever known to the helper
                        && !_helper.AllRemovedObjects.Contains(newObject))
                    // Enumerate
                    .ToArray()
                // Sort them so that we first introduce non-points (lines and circles)
                .OrderBy(newObject => newObject.ObjectType == ConfigurationObjectType.Point ? 1 : 0)
                // Take the first option
                .FirstOrDefault();

            // If there is something to be introduced
            if (newObject != null)
            {
                // Let the normalization helper know
                _helper.IntroduceNewObject(newObject);

                // Add the object to the set of all objects ever introduced
                _allIntroducedObjects.Add(newObject);
            }

            // Set the last introduced object
            _lastIntroducedObject = newObject;

            #endregion

            // Return the removed objects together with the new ones
            return (removedObjects, newObject);
        }

        #endregion
    }
}