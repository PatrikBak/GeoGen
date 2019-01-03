using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoGen.ConsoleTest
{
    public class OutputFormatter
    {
        private Dictionary<int, char> _objectIdToString = new Dictionary<int, char>();

        private Configuration _configuration;

        public OutputFormatter(Configuration configuration)
        {
            _configuration = configuration;
            Register(configuration.ObjectsMap.AllObjects);
        }

        public string FormatConfiguration()
        {
            var looseObjects = _configuration.LooseObjectsHolder.LooseObjects;

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"Loose: {string.Join(", ", looseObjects.Select(ObjectToStringById))}\n");

            foreach (var constructedObject in _configuration.ConstructedObjects)
            {
                stringBuilder.Append($"{ObjectToStringById(constructedObject)} = {ConstructedObjectToString(constructedObject)}\n");
            }

            return stringBuilder.ToString().Trim();
        }

        public void Register(IEnumerable<ConfigurationObject> objects)
        {
            foreach (var configurationObject in objects)
            {
                var id = configurationObject.Id;

                if (_objectIdToString.ContainsKey(id))
                    continue;

                var newLetter = (char) ('A' + _objectIdToString.Count);

                _objectIdToString.Add(id, newLetter);
            }
        }

        public string FormatTheorems(List<Theorem> theorems)
        {
            var stringBuilder = new StringBuilder();

            foreach (var theorem in theorems)
            {
                stringBuilder.Append(ConvertTheoremToString(theorem)).Append("\n");
            }

            return stringBuilder.ToString();
        }

        public string ConvertTheoremToString(Theorem theorem)
        {
            if (theorem.Type == TheoremType.EqualLineSegments || theorem.Type == TheoremType.EqualAngles)
            {
                var firstTwo = theorem.InvolvedObjects.Take(2).Select(TheoremObjectToString).ToList();
                firstTwo.Sort();

                var secondTwo = theorem.InvolvedObjects.Skip(2).Select(TheoremObjectToString).ToList();
                secondTwo.Sort();

                var firstPart = $"[{ string.Join(", ", firstTwo)}]";
                var secondPart = $"[{string.Join(", ", secondTwo)}]";

                return $"{theorem.Type}: {(firstPart.CompareTo(secondPart) < 0 ? firstPart : secondPart)} {(firstPart.CompareTo(secondPart) < 0 ? secondPart : firstPart)}";
            }

            var list = theorem.InvolvedObjects.Select(TheoremObjectToString).ToList();

            list.Sort();

            return $"{theorem.Type}: {string.Join(", ", list)}";
        }

        private string ConstructedObjectToString(ConstructedConfigurationObject constructedObject)
        {
            var result = new StringBuilder();

            result.Append(ConstructionName(constructedObject.Construction))
                    .Append("(")
                    .Append(string.Join(", ", constructedObject.PassedArguments.Select(ArgumentToString)))
                    .Append(")");

            return result.ToString();
        }

        private string ConstructionName(Construction construction)
        {
            if (construction is PredefinedConstruction predefinedConstruction)
                return Regex.Match(predefinedConstruction.Type.ToString(), "(.*)From.*").Groups[1].Value;

            return construction.Name;
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

        private string TheoremObjectToString(TheoremObject theoremObject)
        {
            if (theoremObject.Signature == TheoremObjectSignature.SingleObject)
            {
                return ObjectToStringById(theoremObject.InternalObjects.First());
            }

            var isLine = theoremObject.Signature == TheoremObjectSignature.LineGivenByPoints;

            var list = theoremObject.InternalObjects.Select(ObjectToStringById).ToList();

            list.Sort();

            return $"{(isLine ? "[" : "(")}{string.Join(", ", list)}{(isLine ? "]" : ")")}";
        }

        private string ObjectToStringById(ConfigurationObject configurationObject)
        {
            return _objectIdToString[configurationObject.Id].ToString();
        }
    }
}