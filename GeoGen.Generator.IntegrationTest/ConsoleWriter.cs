using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Generator.IntegrationTest
{
    public static class ConsoleWriter
    {
        private static readonly object _lock = new object();

        public static void WriteLine(object o)
        {
            lock (_lock)
            {
                Console.WriteLine(o);
            }
        }

        public static void WriteLine()
        {
            WriteLine("");
        }

        internal static void WriteLine(int v1, int v2, string v3)
        {
            throw new NotImplementedException();
        }
    }
}
