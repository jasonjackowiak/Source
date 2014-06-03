using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    //enforce use of setup and teardown methods for unit tests
    abstract public class Test
    {

        public abstract void SetUp();

        public abstract void TearDown();
    }
}
