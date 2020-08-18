
namespace DataBase.common.messages
{
    public class GeneralMessages
    {
        public const string ERR_EXCEPTION = "Exception({0}):{1}";
        public const string ERR_IS_NULL = "Parameter({0}) is null or emtpy.";
        public const string ERR_IS_NULL_OR_EMPTY = "Parameter({0}) is null or emtpy.";

        #region For Table
        public const string ERR_TABLE_DOES_NOT_EXIST = "Table({0}) does not exist.";
        public const string ERR_ENTITY_EMPTY = "Entity({0}) contains no column with attribute({1}).";
        #endregion

        #region For Column
        
        public const string ERR_COLUMN_DOES_NOT_EXIST = "Column({0}) does not exist.";
        public const string ERR_COLUMN_VALUE_INVALID = "Column({0}) value('{1}') is invalid.";
        public const string ERR_COLUMN_FAIL_CONVERT = "Column({0}) value('{1}') fail to convert to {2}.";
        public const string ERR_COLUMN_SET_NULL_TO_PRIMARY = "{0}:It`s tried to set null or empty!";
        public const string ERR_COLUMN_TRY_TO_CHANGE_PRIMARY = "{0}:Try to change primary key!";
        public const string ERR_COLUMN_IN_ERROR_STATE = "{0}:It`s tried to set value in error state!";
        public const string ERR_FOREIGN_TYPE_INVALID = "Foreign({0}) type('{1}') is invalid.";
        public const string ERR_FOREIGN_ENTITY_TYPE_INVALID = "ForeignEntity({0}) type('{1}') is invalid.";
        #endregion
    }
}
