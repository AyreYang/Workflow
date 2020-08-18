using Database.Entity.Enums;
using System;
using System.Text.RegularExpressions;

namespace Database.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DBColumnAttribute : Attribute
    {
        public DBTYPE Type { get; private set; }
        public int Size1 { get; private set; }
        public int Size2 { get; private set; }
        public bool IsPK { get; private set; }
        public bool Nullable { get; private set; }
        public Func<object> FuncDefaultValue{ get { return ConvertDefaultValue(); } }

        public object DefaultValue { get; private set; }

        public DBColumnAttribute(DBTYPE type, bool isPK = false, bool nullable = false, object defValue = null)
        {
            Type = type;
            IsPK = isPK;
            Nullable = nullable;
            DefaultValue = defValue;
        }
        public DBColumnAttribute(DBTYPE type, int size, bool isPK = false, bool nullable = false, object defValue = null) :this(type, isPK, nullable, defValue)
        {
            Size1 = size;
        }
        public DBColumnAttribute(string name, DBTYPE type, int size1, int size2, bool isPK = false, bool nullable = false, object defValue = null) : this(type, isPK, nullable, defValue)
        {
            Size1 = size1;
            Size2 = size2;
        }

        //public DBColumn CreateDBColumn()
        //{
        //    return new DBColumn(Name, Type, Size1, Size2, IsPK, Nullable, ConvertDefaultValue());
        //}

        private Func<object> ConvertDefaultValue()
        {
            Func<object> func = null;
            if (DefaultValue != null)
            {
                if (DefaultValue is DBColumnDefaultValue)
                {
                    switch ((DBColumnDefaultValue)DefaultValue)
                    {
                        case DBColumnDefaultValue.CURRENT_TIME:
                            func = () => DateTime.Now;
                            break;
                    }
                }
                else
                {
                    func = () => DefaultValue;
                }
            }
            return func;
        }

        
    }
}
