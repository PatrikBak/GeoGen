using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class ConfigurationObjectsMapTest
    {
        public void Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ConfigurationObjectsMap((IEnumerable<ConfigurationObject>) null)
            );
        }

        public void Configuration_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ConfigurationObjectsMap((Configuration) null)
            );
        }
    }
}