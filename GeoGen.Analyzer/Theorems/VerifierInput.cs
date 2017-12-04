using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal class VerifierInput
    {
        private readonly IContextualContainer _container;

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

        public List<LineObject> AllLines
        {
            get => _allLines ?? (_allLines = _container.GetObjects<LineObject>(AllObjects).ToList());
            set => _allLines = value;
        }

        public List<CircleObject> AllCircles
        {
            get => _allCircles ?? (_allCircles = _container.GetObjects<CircleObject>(AllObjects).ToList());
            set => _allCircles = value;
        }

        public List<PointObject> AllPoints
        {
            get => _allPoints ?? (_allPoints = _container.GetObjects<PointObject>(AllObjects).ToList());
            set => _allPoints = value;
        }

        public List<PointObject> NewPoints
        {
            get => _newPoints ?? (_newPoints = _container.GetNewObjects<PointObject>(_oldObjects, _newObjects).ToList());
            set => _newPoints = value;
        }

        public List<LineObject> NewLines
        {
            get => _newLines ?? (_newLines = _container.GetNewObjects<LineObject>(_oldObjects, _newObjects).ToList());
            set => _newLines = value;
        }

        public List<CircleObject> NewCircles
        {
            get => _newCircles ?? (_newCircles = _container.GetNewObjects<CircleObject>(_oldObjects, _newObjects).ToList());
            set => _newCircles = value;
        }

        public List<PointObject> OldPoints
        {
            get => _oldPoints ?? (_oldPoints = _container.GetObjects<PointObject>(_oldObjects).ToList());
            set => _oldPoints = value;
        }

        public List<LineObject> OldLines
        {
            get => _oldLines ?? (_oldLines = _container.GetObjects<LineObject>(_oldObjects).ToList());
            set => _oldLines = value;
        }

        public List<CircleObject> OldCircles
        {
            get => _oldCircles ?? (_oldCircles = _container.GetObjects<CircleObject>(_oldObjects).ToList());
            set => _oldCircles = value;
        }

        public VerifierInput(IContextualContainer container, ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _oldObjects = oldObjects ?? throw new ArgumentNullException(nameof(oldObjects));
            _newObjects = newObjects ?? throw new ArgumentNullException(nameof(newObjects));
            AllObjects = oldObjects.Merge(newObjects);
        }
    }
}