using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Database.ADO.interfaces
{
    public interface IADbCommand
    {
        DbCommand Command { get; }
        Action<DbCommand, List<DbCommand>> CallbackAction { get; }
        long Execute(DbConnection connection, DbTransaction transaction, List<DbCommand> commands = null);
    }
}
