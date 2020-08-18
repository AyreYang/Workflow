using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DataBase.common.enums;
using DataBase.common.messages;

namespace DataBase.common.objects
{
    public class DBColumn
    {
        private object _value1 = null;
        private object _value0 = null;
        private object _iniValue = null;

        public ColumnState State { get; private set; }
        public string ID { get; private set; }
        public string DBType { get; private set; }
        public Type DataType { get; private set; }
        public string DBTypeString
        {
            get
            {
                string type = null;
                //switch (DataType.ToString())
                //{
                //    case 
                //}
                return type;
            }
        }
        public KeyType KeyType { get; private set; }
        public bool Nullable { get; private set; }
        public string Foreign { get; private set; }
        //public bool Ignorable { get; private set; }
        //public bool Editable { get; private set; }
        public object DefaultValue { get; private set; }
        public object Value
        {
            get
            {
                return _value1;
            }
            set
            {
                switch (State)
                {
                    case ColumnState.ERROR:
                        break;
                    case ColumnState.RAW:
                        if (SetValue(value) == true)
                        {
                            State = ColumnState.ASSIGNED;
                        }
                        break;
                    case ColumnState.ASSIGNED:
                        SetValue(value);
                        break;
                    case ColumnState.FRESHED:
                        if (SetValue(value) == true)
                        {
                            if (_value1 != _value0)
                            {
                                State = ColumnState.CHANGED;
                            }
                        }
                        break;
                    case ColumnState.CHANGED:
                        if (SetValue(value) == true)
                        {
                            if (_value1 == _value0)
                            {
                                State = ColumnState.FRESHED;
                            }
                        }
                        break;
                }
            }
        }
        public object Value0
        {
            get
            {
                return _value0;
            }
        }

        internal DBColumn(string id, string dbtype, Type field, KeyType key, bool nullable, object def, string foreign)
        {
            ID = id.Trim().ToUpper();
            DBType = dbtype;
            DataType = field;
            KeyType = key;
            Nullable = (key == KeyType.Primary) ? false : nullable;
            DefaultValue = def;
            Foreign = foreign;

            State = ColumnState.RAW;

            _iniValue = DefaultValue;
            _value1 = _iniValue;
            _value0 = _iniValue;

        }

        public DBColumn Clone()
        {
            return new DBColumn(this.ID, this.DBType, this.DataType, this.KeyType, this.Nullable, this.DefaultValue, this.Foreign)
            {
                _value0 = this._value0,
                _value1 = this._value1,
                _iniValue = this._iniValue,
                State = this.State
            };
        }


        public void Reset()
        {
            _value1 = _iniValue;
            _value0 = _iniValue;
            State = ColumnState.RAW;
        }

        public void Fresh(DataGridViewRow adr_row)
        {
            if (State < ColumnState.RAW) return;
            if (adr_row == null || adr_row.Cells == null) return;
            IEnumerable<DataGridViewColumn> columns = adr_row.DataGridView.Columns.Cast<DataGridViewColumn>();
            if (!columns.Any<DataGridViewColumn>(column => (ID.Equals(column.Name)))) return;
            object value = adr_row.Cells[ID].Value;
            Reset();
            if (SetValue(value))
            {
                _value0 = value;
                State = ColumnState.FRESHED;
            }
        }

        public void Fresh(DataRow adr_row)
        {
            if (State < ColumnState.RAW) return;
            if (adr_row == null || adr_row.Table.Columns == null || !adr_row.Table.Columns.Contains(ID)) return;
            //object value = DatabaseCore.GetValueFormDataRow<object>(adr_row, this.ID);
            object value = adr_row[this.ID];
            Reset();
            if (SetValue(value))
            {
                _value0 = value;
                State = ColumnState.FRESHED;
            }
        }

        private bool SetValue(object value)
        {
            var result = false;
            object val = null;
            if (KeyType.Primary.Equals(KeyType))
            {
                if (this.State != ColumnState.RAW && this.State != ColumnState.ASSIGNED)
                {
                    throw new Exception(string.Format(GeneralMessages.ERR_COLUMN_TRY_TO_CHANGE_PRIMARY, this.ID));
                }

                var str = DatabaseCore.Convert2<string>(value, out result);
                if(!result || string.IsNullOrWhiteSpace(str))
                {
                    throw new Exception(string.Format(GeneralMessages.ERR_COLUMN_SET_NULL_TO_PRIMARY, this.ID));
                }
            }
            if (State == ColumnState.ERROR)
            {
                throw new Exception(string.Format(GeneralMessages.ERR_COLUMN_IN_ERROR_STATE, this.ID));
            }

            if (value == DBNull.Value)
            {
                value = null;
            }
            if (value == null)
            {
                _value1 = null;
                result = true;
            }
            else
            {
                if (this.DataType.Equals(value.GetType()))
                {
                    _value1 = value;
                    result = true;
                }
                else
                {
                    if (this.DataType == typeof(string)) val = DatabaseCore.Convert2<string>(value, out result);
                    if (this.DataType == typeof(int) || this.DataType == typeof(int?)) val = DatabaseCore.Convert2<int>(value, out result);
                    if (this.DataType == typeof(long) || this.DataType == typeof(long?)) val = DatabaseCore.Convert2<long>(value, out result);
                    if (this.DataType == typeof(decimal) || this.DataType == typeof(decimal?)) val = DatabaseCore.Convert2<decimal>(value, out result);
                    if (this.DataType == typeof(DateTime) || this.DataType == typeof(DateTime?)) val = DatabaseCore.Convert2<DateTime>(value, out result);
                    if (result) _value1 = val;
                }
            }
            return result;
        }

    }
}
