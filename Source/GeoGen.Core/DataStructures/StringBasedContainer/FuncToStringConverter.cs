using System;

namespace GeoGen.Core
{
    public class FuncToStringConverter<T> : IToStringConverter<T>
    {
        private readonly Func<T, string> _function;

        public FuncToStringConverter(Func<T, string> function)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
        }

        public string ConvertToString(T item) => _function(item);
    }
}
