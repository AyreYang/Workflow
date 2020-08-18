using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataBase.common.enums;
using DataBase.common.objects;

namespace DataBase.common.attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DBFieldAttribute : Attribute
    {
        private const string PATTERN0 = "[{0}]";
        private const string PATTERN1 = "[{0}]({1})";
        private const string PATTERN2 = "[{0}]({1},{2})";

        private readonly static Dictionary<string, Type> DBTYPE_MAPPING1 = new Dictionary<string, Type>()
        {
            {"uniqueidentifier", typeof(Guid?)},
            {"char", typeof(string)},
            {"nchar", typeof(string)},
            {"varchar", typeof(string)},
            {"nvarchar", typeof(string)},
            {"text", typeof(string)},
            {"ntext", typeof(string)},
            {"int", typeof(int?)},
            {"bigint", typeof(long?)},
            {"smallint", typeof(int?)},
            {"tinyint", typeof(int?)},
            {"float", typeof(float?)},
            {"decimal", typeof(decimal?)},
            {"numeric", typeof(decimal?)},
            {"money", typeof(decimal?)},
            {"smallmoney", typeof(decimal?)},
            {"bit", typeof(bool?)},
            {"binary", typeof(byte[])},
            {"image", typeof(byte[])},
            {"date", typeof(DateTime?)},
            {"time", typeof(DateTime?)},
            {"datetime", typeof(DateTime?)},
            {"timestamp", typeof(long?)}
        };
        private readonly static Dictionary<string, string> DBTYPE_MAPPING2 = new Dictionary<string, string>()
        {
            {"guid", "uniqueidentifier"},
            {"string", "nvarchar"},
            {"int", "int"},
            {"long", "bigint"},
            {"float", "float"},
            {"decimal", "decimal"},
            {"byte", "bit"},
            {"bool", "bit"},
            {"byte[]", "binary"},
            {"datetime", "datetime"}
        };

        public string Name { get; private set; }
        public string DBType { get; private set; }
        public Type FieldType { get; private set; }
        public KeyType KeyType { get; private set; }
        public bool Nullable { get; private set; }
        public object DefaultValue { get; private set; }
        public string Foreign { get; private set; }


        public bool IsValid
        {
            get
            {
                return Regex.IsMatch(Name ?? string.Empty, @"^[_a-zA-Z0-9]+$");
            }
        }

        public DBFieldAttribute(string name, string type, KeyType key, bool nullable, object def, string foreign)
        {
            if(string.IsNullOrWhiteSpace(name))throw new ArgumentNullException("name", "parameter(name) is null or empty!");
            if (!Regex.IsMatch(name.Trim(), @"^[_a-zA-Z0-9]+$")) throw new ArgumentException(string.Format("parameter(name:{0}) is invalid!", name));
            Name = name.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException("type", "parameter(type) is null or empty!");
            if (!SetType(type)) throw new ArgumentException(string.Format("parameter(type:{0}) is invalid!", type));

            KeyType = key;
            Nullable = nullable;
            DefaultValue = def;
            Foreign = string.IsNullOrWhiteSpace(foreign) ? null : foreign.Trim();
        }
        public DBFieldAttribute(string name, string type) : this(name, type, enums.KeyType.Normal, true, null, null) { }
        public DBFieldAttribute(string name, string type, KeyType key) : this(name, type, key, true, null, null) { }
        public DBFieldAttribute(string name, string type, KeyType key, bool nullable) : this(name, type, key, nullable, null, null) { }
        public DBFieldAttribute(string name, string type, KeyType key, bool nullable, object def) : this(name, type, key, nullable, def, null) { }

        public DBFieldAttribute(string name, Type type, KeyType key, bool nullable, object def, string foreign) : this(name, type.ToString(), key, nullable, def, foreign) { }
        public DBFieldAttribute(string name, Type type) : this(name, type.ToString(), enums.KeyType.Normal, true, null, null) { }
        public DBFieldAttribute(string name, Type type, KeyType key) : this(name, type.ToString(), key, true, null, null) { }
        public DBFieldAttribute(string name, Type type, KeyType key, bool nullable) : this(name, type.ToString(), key, nullable, null, null) { }
        public DBFieldAttribute(string name, Type type, KeyType key, bool nullable, object def) : this(name, type.ToString(), key, nullable, def, null) { }

        


        private string GetType0(string stype)
        {
            string type = null;
            var pattern = @"\[(?<type>[a-zA-Z]+)\]";
            var mc = new Regex(pattern).Matches(stype);
            if (mc.Count > 0)
            {
                Match m = mc[0];
                var tp = m.Groups["type"].Value.Replace("[", string.Empty).Replace("]", string.Empty).ToLower();
                if (DBTYPE_MAPPING1.ContainsKey(tp)) type = tp;
            }
            return type;
        }
        private string GetType1(string stype, out object parameter1)
        {
            parameter1 = null;
            var pattern1 = @"\((?<p1>\d+)\)";
            var pattern2 = @"\((?<p1>[a-zA-Z]+)\)";
            string type = GetType0(stype);
            var mc = new Regex(pattern1).Matches(stype);
            if (mc.Count > 0)
            {
                Match m = mc[0];
                var p1 = m.Groups["p1"].Value;

                parameter1 = int.Parse(p1);
            }
            else
            {
                mc = new Regex(pattern2).Matches(stype);
                if (mc.Count > 0)
                {
                    Match m = mc[0];
                    parameter1 = m.Groups["p1"].Value;
                }
            }
            return type;
        }
        private string GetType2(string stype, out object parameter1, out object parameter2)
        {
            parameter1 = null;
            parameter2 = null;
            var pattern = @"\((?<p1>\d+)\,(?<p2>\d+)\)";
            string type = GetType0(stype);
            var mc = new Regex(pattern).Matches(stype);
            if (mc.Count > 0)
            {
                Match m = mc[0];
                var p1 = m.Groups["p1"].Value;
                var p2 = m.Groups["p2"].Value;

                parameter1 = int.Parse(p1);
                parameter2 = int.Parse(p2);
            }
            return type;
        }
        private bool GetType(string type, out string dbtype, out object parameter1, out object parameter2, out int mode)
        {
            dbtype = null;
            parameter1 = null;
            parameter2 = null;
            mode = 0;
            var result = false;

            var stype = type.Replace(" ", string.Empty).ToLower();

            // without parameter
            if (Regex.IsMatch(stype, @"^\[[a-zA-Z]+\]$"))
            {
                mode = 0;
                dbtype = GetType0(stype);
                result = !string.IsNullOrWhiteSpace(dbtype);
                return result;
            }
            // with 1 parameter
            if (Regex.IsMatch(stype, @"^\[[a-zA-Z]+\]\([0-9]+\)$"))
            {
                mode = 1;
                dbtype = GetType1(stype, out parameter1);
                result = !string.IsNullOrWhiteSpace(dbtype);
                return result;
            }
            // with 2 parameters
            if (Regex.IsMatch(stype, @"^\[[a-zA-Z]+\]\([0-9]+\,[0-9]+\)$"))
            {
                mode = 2;
                dbtype = GetType2(stype, out parameter1, out parameter2);
                result = !string.IsNullOrWhiteSpace(dbtype);
                return result;
            }

            if (Regex.IsMatch(stype, @"^[_a-zA-Z]{1}[_a-zA-Z0-9]*(\.[_a-zA-Z]{1}[_a-zA-Z0-9]*)*$"))
            {
                if (typeof(Guid).ToString().ToLower().Equals(stype) || "guid".Equals(stype))
                {
                    result = true;
                    mode = 0;
                    dbtype = DBTYPE_MAPPING2["guid"];
                    return result;
                }
                if (typeof(string).ToString().ToLower().Equals(stype) || "string".Equals(stype))
                {
                    result = true;
                    mode = 1;
                    dbtype = DBTYPE_MAPPING2["string"];
                    parameter1 = "MAX";
                    return result;
                }
                if (typeof(int).ToString().ToLower().Equals(stype) || "int".Equals(stype))
                {
                    result = true;
                    mode = 0;
                    dbtype = DBTYPE_MAPPING2["int"];
                    return result;
                }
                if (typeof(long).ToString().ToLower().Equals(stype) || "long".Equals(stype))
                {
                    result = true;
                    mode = 0;
                    dbtype = DBTYPE_MAPPING2["long"];
                    return result;
                }
                if (typeof(decimal).ToString().ToLower().Equals(stype) || "decimal".Equals(stype))
                {
                    result = true;
                    mode = 2;
                    dbtype = DBTYPE_MAPPING2["decimal"];
                    parameter1 = 18;
                    parameter2 = 4;
                    return result;
                }
                if (typeof(DateTime).ToString().ToLower().Equals(stype) || "datetime".Equals(stype))
                {
                    result = true;
                    mode = 0;
                    dbtype = DBTYPE_MAPPING2["datetime"];
                    return result;
                }
            }
            return result;
        }
        private bool SetType(string type)
        {
            string dbtype = null;
            object parameter1 = null;
            object parameter2 = null;
            int mode = -1;
            var result = GetType(type, out dbtype, out parameter1, out parameter2, out mode);
            if (result)
            {
                switch (mode)
                {
                    case 0:
                        this.DBType = string.Format(PATTERN0, dbtype);
                        this.FieldType = DBTYPE_MAPPING1[dbtype];
                        break;
                    case 1:
                        this.DBType = string.Format(PATTERN1, dbtype, parameter1);
                        this.FieldType = DBTYPE_MAPPING1[dbtype];
                        break;
                    case 2:
                        this.DBType = string.Format(PATTERN2, dbtype, parameter1, parameter2);
                        this.FieldType = DBTYPE_MAPPING1[dbtype];
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }

        //public DBFieldAttribute(string name, string foreign, DBTYPEContainer dbtype, KeyType key, bool nullable, object def)
        //{
        //    Name = (!string.IsNullOrWhiteSpace(name)) ? name.Trim().ToUpper() : string.Empty;
        //    DBType = null;
        //    KeyType = key;
        //    Nullable = nullable;
        //    DefaultValue = def;
        //    Foreign = foreign;
        //}
        //public DBFieldAttribute(string name, string foreign, DBTYPEContainer dbtype, KeyType key, bool nullable) : this(name, foreign, dbtype, key, nullable, null) { }
        //public DBFieldAttribute(string name, DBTYPEContainer dbtype, KeyType key, bool nullable) : this(name, null, dbtype, key, nullable, null) { }
        //public DBFieldAttribute(string name, DBTYPEContainer dbtype, KeyType key) : this(name, null, dbtype, key, true, null) { }
        //public DBFieldAttribute(string name, DBTYPEContainer dbtype, KeyType key, object def) : this(name, null, dbtype, key, true, def) { }
        //public DBFieldAttribute(string name, DBTYPEContainer dbtype, KeyType key, bool nullable, object def) : this(name, null, dbtype, key, nullable, def) { }
        //public DBFieldAttribute(string name, Type field, bool nullable, string foreign = null) : this(name, field, KeyType.Normal, nullable, null, foreign) { }

        //public DBFieldAttribute(string name, Type field, object def, string foreign = null) : this(name, field, KeyType.Normal, true, def, foreign) { }
        //public DBFieldAttribute(string name, DBTYPEContainer dbtype) : this(name, null, dbtype, KeyType.Normal, true, null) { }

        //private string CreateDBTypeString(DataBase.common.enums.DBType type, object[] argums)
        //{
        //    var str0 = "[{0}]";
        //    string stype = null;
        //    switch (type)
        //    {
        //        case DataBase.common.enums.DBType.BIGINT:
        //            stype = 
        //    }
        //}

        public DBColumn CreateDBColumn()
        {
            return new DBColumn(Name, DBType, FieldType, KeyType, Nullable, DefaultValue, Foreign);
        }
    }
}
