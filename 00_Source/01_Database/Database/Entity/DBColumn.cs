using Database.Entity.Enums;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Database.Entity
{
    internal enum DBColumnRAW
    {
        VALUE = 0
    }
    internal class DBColumn
    {
        private TableEntity _host = null;
        private PropertyInfo _pinfo = null;

        public object Value
        {
            get
            {
                return _pinfo.GetValue(_host);
            }
        }
        private object _value0 { get; set; }
        public object Value0
        {
            get
            {
                return _value0;
            }
        }

        private STATUS _status0 { get; set; }
        public STATUS Status
        {
            get
            {
                return _status0 == STATUS.ERROR ? STATUS.ERROR : (STATUS)(((int)_status0) + (Changed ? 1 : 0));
            }
        }

        public bool Changed
        {
            get
            {
                var val0 = Value0;
                var val1 = Value;

                if (val0 != null && val1 != null)
                {
                    return !val1.Equals(val0);
                }
                else
                {
                    if (val0 != null)
                    {
                        return !val0.Equals(val1);
                    }
                    if (val1 != null)
                    {
                        return !val1.Equals(val0);
                    }

                    return false;
                }
            }
        }

        public string Name { get; private set; }
        public string TableName
        {
            get
            {
                return _host.Schema != null ? _host.Schema.Name : _host.GetType().Name;
            }
        }
        public string FullName
        {
            get
            {
                return string.Format("[{0}].[{1}]", TableName, this.Name);
            }
        }
        public DBTYPE Type { get; private set; }
        public int Size1 { get; private set; }
        public int Size2 { get; private set; }
        public bool IsPK { get; private set; }
        public bool Nullable { get; private set; }

        public Func<object> FuncDefaultValue { get; private set; }

        internal DBColumn(TableEntity host, PropertyInfo pinfo, DBTYPE type, int size1, int size2, bool isPK = false, bool nullable = false, Func<object> defValue = null)
        {
            if (host == null) throw new ArgumentNullException("DBColumn(host) is null!");
            if (pinfo == null) throw new ArgumentNullException("DBColumn(pinfo) is null!");

            _host = host;
            _pinfo = pinfo;

            Name = _pinfo.Name;
            Type = type;
            Size1 = size1;
            Size2 = size2;
            IsPK = isPK;
            Nullable = nullable;
            FuncDefaultValue = defValue;

            _value0 = DBColumnRAW.VALUE;
            _status0 = STATUS.RAW;

            CheckName();
            CheckDBTYPE();
            CheckNullable();
        }

        private void CheckName()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ApplicationException("DBColumn(name) is null or empty!");
            if (!Regex.IsMatch(Name ?? string.Empty, @"^[_a-zA-Z][_a-zA-Z0-9]+$")) throw new ApplicationException(string.Format("DBColumn(name:{0}) is illegal!", Name));
        }
        private void CheckDBTYPE()
        {
            switch (Type)
            {
                case DBTYPE.VARCHAR:
                case DBTYPE.NVARCHAR:
                    if (Size2 != 0) throw new ApplicationException(string.Format("DBColumn({0}) has illegal type({1}({2},{3}))!", FullName, Type.ToString(), Size1, Size2));
                    if (Size1 <= 0) throw new ApplicationException(string.Format("DBColumn({0}) has illegal type({1}({2}))!", FullName, Type.ToString(), Size1));
                    break;
                case DBTYPE.DECIMAL:
                case DBTYPE.NUMERIC:
                    if (Size1 <= 0 || Size2 < 0) throw new ApplicationException(string.Format("DBColumn({0}) has illegal type({1}({2},{3}))!", FullName, Type.ToString(), Size1, Size2));
                    break;

            }
        }
        private void CheckNullable()
        {
            if (IsPK && Nullable) throw new ApplicationException(string.Format("DBColumn({0}) is primary key, but nullable!", FullName));
        }

        internal void SetValue(object value)
        {
            _pinfo.SetValue(_host, value);
        }
        internal void SetValue0(object value)
        {
            _value0 = value;
            _status0 = STATUS.FRESHED;
        }
    }
}
