
namespace DataBase.common.enums
{
    public enum EntityState
    {
        ERROR = -99,
        DELETED = -2,
        UNKNOWN = -1,
        RAW = 0,
        ASSIGNED = 1,
        FRESHED = 2,
        CHANGED = 3
    }

    #region Column Enums
    public enum ColumnState
    {
        ERROR = -99,
        RAW = 0,
        ASSIGNED = 1,
        FRESHED = 2,
        CHANGED = 3
    }
    public enum KeyType
    {
        Normal = 0,
        Primary = 1,
        IncrementPrimary = 2
    }
    public enum ForeignMode
    {
        Reference = 0,
        Correlative = 1
    }
    //public enum Sort
    //{
    //    asc, desc
    //}
    public enum TableSchamaType
    {
        TABLE,
        VIEW,
        CUSTOMIZE
    }
    #endregion
}
