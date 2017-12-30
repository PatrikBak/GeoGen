using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Theorems;

namespace GeoGen.Generator.IntegrationTest
{
    internal class OutputFormatter
    {
        private Dictionary<int, char> _objectIdToString;

        public string Format(Configuration configuration)
        {
            _objectIdToString = new Dictionary<int, char>();
            var looseObjects = configuration.LooseObjects;
            Register(looseObjects);
            Register(configuration.ConstructedObjects);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"Loose: {string.Join(", ", looseObjects.Select(ObjectToStringById))}\n");

            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                stringBuilder.Append($"{ObjectToStringById(constructedObject)} = {ConstructedObjectToString(constructedObject)}\n");
            }

            return stringBuilder.ToString().Trim();
        }

        private string ConstructedObjectToString(ConstructedConfigurationObject constructedObject)
        {
            var result = new StringBuilder();

            result.Append(ConstructionToString(constructedObject.Construction))
                    .Append("(")
                    .Append(string.Join(", ", constructedObject.PassedArguments.Select(ArgumentToString)))
                    .Append(")");

            return result.ToString();
        }

        private string ConstructionToString(Construction construction)
        {
            if (construction is PredefinedConstruction)
                return construction.GetType().Name;

            return ((ComposedConstruction) construction).Name;
        }

        private string ArgumentToString(ConstructionArgument argument)
        {
            if (argument is ObjectConstructionArgument objectArgument)
            {
                return ObjectToStringById(objectArgument.PassedObject);
            }

            var setArgument = (SetConstructionArgument) argument;

            return $"{{{string.Join(", ", setArgument.PassedArguments.Select(ArgumentToString))}}}";
        }

        public string Format(List<Theorem> theorems)
        {
            var stringBuilder = new StringBuilder();

            foreach (var theorem in theorems)
            {
                stringBuilder.Append(ConvertToString(theorem)).Append("\n");
            }

            return stringBuilder.ToString();
        }

        private string ConvertToString(Theorem theorem)
        {
            return $"{theorem.Type}: {string.Join(", ", theorem.InvolvedObjects.Select(ObjectToString))}";
        }

        private string ObjectToString(TheoremObject theoremObject)
        {
            if (theoremObject.Type == TheoremObjectSignature.SingleObject)
            {
                return ObjectToStringById(theoremObject.InternalObjects.First());
            }

            var isLine = theoremObject.Type == TheoremObjectSignature.LineGivenByPoints;

            return $"{(isLine ? "[" : "(")}{string.Join(", ", theoremObject.InternalObjects.Select(ObjectToStringById))}{(isLine ? "]" : ")")}";
        }

        private string ObjectToStringById(ConfigurationObject configurationObject)
        {
            return _objectIdToString[configurationObject.Id ?? throw new Exception()].ToString();
        }

        private void Register(IEnumerable<ConfigurationObject> objects)
        {
            foreach (var configurationObject in objects)
            {
                var id = configurationObject.Id ?? throw new Exception();

                if (_objectIdToString.ContainsKey(id))
                    continue;

                var newLetter = (char) ('A' + _objectIdToString.Count);

                _objectIdToString.Add(id, newLetter);
            }
        }
    }
}