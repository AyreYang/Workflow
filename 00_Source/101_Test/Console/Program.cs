using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var approvers = Rule4Model2.Rules.FindApprovers(null);
            return;
        }
    }
}
