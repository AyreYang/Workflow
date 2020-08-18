using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entity.Enums
{
    public enum DBTYPE
    {
        INT,
        INTEGER,

        BIT,
        BOOLEAN,

        BLOB,

        DATETIME,

        NUMERIC,
        DECIMAL,

        UNIQID,
        VARCHAR,
        NVARCHAR

    }
    public enum STATUS
    {
        RAW = 0,
        ASSIGNED = 1,
        FRESHED = 2,
        CHANGED = 3,
        ERROR = 4
    }

    public enum DBColumnDefaultValue
    {
        CURRENT_TIME
    }
}
