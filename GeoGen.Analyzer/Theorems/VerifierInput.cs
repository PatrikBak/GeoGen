using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer
{
    internal class VerifierInput
    {
        public readonly IContextualContainer _container;

        private readonly ConfigurationObjectsMap _oldObjects;

        private readonly ConfigurationObjectsMap _newObjects;

        private List<LineObject> _allLines;

        private List<CircleObject> _allCircles;

        private List<PointObject> _allPoints;

        private List<PointObject> _newPoints;

        private List<LineObject> _newLines;

        private List<CircleObject> _newCircles;

        private List<PointObject> _oldPoints;

        private List<CircleObject> _oldCircles;

        private List<LineObject> _oldLines;

        public ConfigurationObjectsMap AllObjects { get; }

        // TODO: CLEAN UP (merge All / Old methods to one generic method)

        public List<LineObject> AllLines => _allLines ?? (_allLines = _container.GetObjects<LineObject>(AllObjects).ToList());

        public List<CircleObject> AllCircles => _allCircles ?? (_allCircles = _container.GetObjects<CircleObject>(AllObjects).ToList());

        public List<PointObject> AllPoints => _allPoints ?? (_allPoints = _container.GetObjects<PointObject>(AllObjects).ToList());

        public List<PointObject> NewPoints => _newPoints ?? (_newPoints = _container.GetNewObjects<PointObject>(_oldObjects, _newObjects).ToList());

        public List<LineObject> NewLines => _newLines ?? (_newLines = _container.GetNewObjects<LineObject>(_oldObjects, _newObjects).ToList());

        public List<CircleObject> NewCircles => _newCircles ?? (_newCircles = _container.GetNewObjects<CircleObject>(_oldObjects, _newObjects).ToList());

        public List<PointObject> OldPoints => _oldPoints ?? (_oldPoints = _container.GetObjects<PointObject>(_oldObjects).ToList());

        public List<LineObject> OldLines => _oldLines ?? (_oldLines = _container.GetObjects<LineObject>(_oldObjects).ToList());

        public List<CircleObject> OldCircles => _oldCircles ?? (_oldCircles = _container.GetObjects<CircleObject>(_oldObjects).ToList());

        public VerifierInput(IContextualContainer container, ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _oldObjects = oldObjects ?? throw new ArgumentNullException(nameof(oldObjects));
            _newObjects = newObjects ?? throw new ArgumentNullException(nameof(newObjects));
            AllObjects = oldObjects.Merge(newObjects);
        }
    }
}