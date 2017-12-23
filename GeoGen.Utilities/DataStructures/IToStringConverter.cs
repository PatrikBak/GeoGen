using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Utilities.DataStructures
{
    public interface IToStringConverter<T>
    {
        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The string representation.</returns>
        string ConvertToString(T item);
    }
}
