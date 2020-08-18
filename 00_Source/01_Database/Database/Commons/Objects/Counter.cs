using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Commons.Objects
{
    public class Counter
    {
        public int Value { get; private set; }

        public Counter(int count = 0)
        {
            Value = count > 0 ? count : 0;
        }
        public int Count()
        {
            return ++Value;
        }
    }
}
