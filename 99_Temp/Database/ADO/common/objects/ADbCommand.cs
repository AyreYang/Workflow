using System;
using System.Collections.Generic;
using System.Data.Common;
using Database.ADO.interfaces;

namespace DataBase.common.objects
{
    class ADbCommand : IADbCommand
    {
        public ADbCommand(DbCommand command, Action<DbCommand, List<DbCommand>> action = null)
        {
            Command = command;
            CallbackAction = action;
        }

        public DbCommand Command
        {
            get;
            private set;
        }

        public Action<DbCommand, List<DbCommand>> CallbackAction
        {
            get;
            private set;
        }


        public long Execute(DbConnection connection, DbTransaction transaction, List<DbCommand> commands = null)
        {
            var ret = 0;
            if (Command != null)
                using (Command)
            {
                
                Command.Connection = connection;
                if (transaction != null) Command.Transaction = transaction;
                ret = Command.ExecuteNonQuery();
            }
            if (CallbackAction != null) CallbackAction(Command, commands);
            return ret;
        }
    }
}
