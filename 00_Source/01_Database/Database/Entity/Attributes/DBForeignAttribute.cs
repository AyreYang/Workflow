using Database.Entity.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Database.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DBForeignAttribute : Attribute
    {
        private const string Separater = "=>";
        public ReadOnlyPair<string, string>[] Keys { get; private set; }
        public bool Initial { get; private set; }
        public DBForeignAttribute(params string[] keys)
        {
            Keys = Convert(keys);
            Initial = true;
        }
        public DBForeignAttribute(bool initial, params string[] keys):this(keys)
        {
            Initial = initial;
        }

        private ReadOnlyPair<string, string>[] Convert(params string[] keys)
        {
            var list = new List<ReadOnlyPair<string, string>>();
            if (keys != null && keys.Length > 0)
            {
                if (keys.Any(k => !Regex.IsMatch(k, string.Format(@"^[_a-zA-Z0-9]+{0}[_a-zA-Z0-9]+$", Separater))))
                {
                    throw new ApplicationException(string.Format("Foreign key matching format is illgal! Patter:Key1{0}Key2", Separater));
                }
                foreach (var key in keys.Distinct())
                {
                    var array = key.Split(Separater.ToCharArray());
                    list.Add(new ReadOnlyPair<string, string>(array.First(), array.Last()));
                }
            }

            return list.ToArray();
        }
    }
}
