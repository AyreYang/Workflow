using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.common.enums;

namespace DataBase.common.attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DBForeignAttribute : Attribute
    {
        public const char saparator = ':';

        public string TableName { get; private set; }
        public ForeignMode Mode { get; private set; }
        public List<KeyValuePair<string, string>> Keys { get; private set; }
        public bool IsValid
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(TableName)) && (Keys.Count > 0);
            }
        }

        
        public DBForeignAttribute(string table, ForeignMode mode, params string[] externals)
        {
            TableName = (!string.IsNullOrWhiteSpace(table)) ? table.Trim().ToUpper() : string.Empty;
            Mode = mode;
            Keys = new List<KeyValuePair<string, string>>();

            if (externals != null && externals.Length > 0) 
                foreach (string external in externals)
                {
                    if (string.IsNullOrWhiteSpace(external)) continue;
                    var raw = external.Trim().ToUpper();
                    if (raw.Contains(saparator))
                    {
                        var columns = raw.Split(new char[] { saparator });
                        var column1 = columns[0].Trim();
                        var column2 = columns[1].Trim();
                        if (string.IsNullOrWhiteSpace(column1) || string.IsNullOrWhiteSpace(column2)) continue;
                        Keys.Add(new KeyValuePair<string, string>(column1, column2));
                    }
                    else
                    {
                        Keys.Add(new KeyValuePair<string, string>(raw, raw));
                    }
                }
        }
        public DBForeignAttribute(string table, params string[] externals)
            : this(table, ForeignMode.Reference, externals) { }
    }
}
