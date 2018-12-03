using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoGen.Core;

namespace GeoGen.Generator.IntegrationTest
{
    internal class OutputFormatter
    {
        private Dictionary<int, char> _objectIdToString;

        private ConstructionsContainer _constructionsContainer;

        public OutputFormatter(ConstructionsContainer constructionsContainer)
        {
            _constructionsContainer = constructionsContainer;
        }

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
            return _constructionsContainer.GetName(construction);
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

        public string ConvertToString(Theorem theorem)
        {
            if (theorem.Type == TheoremType.EqualLineSegments)
            {
                var firstTwo = theorem.InvolvedObjects.Take(2).Select(ObjectToString).ToList();
                firstTwo.Sort();

                var secondTwo = theorem.InvolvedObjects.Skip(2).Select(ObjectToString).ToList();
                secondTwo.Sort();

                return $"{theorem.Type}: [{string.Join(", ", firstTwo)}] [{string.Join(", ", secondTwo)}]";
            }

            var list = theorem.InvolvedObjects.Select(ObjectToString).ToList();

            list.Sort();

            return $"{theorem.Type}: {string.Join(", ", list)}";
        }

        private string ObjectToString(TheoremObject theoremObject)
        {
            if (theoremObject.Type == TheoremObjectSignature.SingleObject)
            {
                return ObjectToStringById(theoremObject.InternalObjects.First());
            }

            var isLine = theoremObject.Type == TheoremObjectSignature.LineGivenByPoints;

            var list = theoremObject.InternalObjects.Select(ObjectToStringById).ToList();

            list.Sort();

            return $"{(isLine ? "[" : "(")}{string.Join(", ",list)}{(isLine ? "]" : ")")}";
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