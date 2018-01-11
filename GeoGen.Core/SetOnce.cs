using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Core
{
    public class WriteOnce<T>
    {
        private T _value;

        private bool _valueIsSet;

        public T Value
        {
            get
            {
                if (!_valueIsSet)
                    throw new InvalidOperationException("Value of the property haven't been set yet.");

                return _value;
            }
            set
            {
                if (_valueIsSet)
                    throw new InvalidOperationException("Value of the property can be set at most once.");

                _value = value;
                _valueIsSet = true;
            }
        }

        public static implicit operator T(WriteOnce<T> toConvert)
        {
            return toConvert.Value;
        }
    }
}