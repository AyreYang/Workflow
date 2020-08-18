using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Database.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DBTableAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool Readonly { get; private set; }

        public DBTableAttribute(string name, bool readOnly = false)
        {
            Name = name.Trim();
            CheckName();
            Readonly = readOnly;
        }
        private void CheckName()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ApplicationException("DBTable(name) is null or empty!");
            if (!Regex.IsMatch(Name ?? string.Empty, @"^[_a-zA-Z][_a-zA-Z0-9]+$")) throw new ApplicationException(string.Format("DBTable(name:{0}) is illegal!", Name));
        }
    }
}
