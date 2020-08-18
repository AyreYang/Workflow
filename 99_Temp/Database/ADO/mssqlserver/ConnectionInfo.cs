using System;
using DataBase.common.interfaces;
using DataBase.common.messages;
using ServiceCore.Encryption.interfaces;
using ServiceCore.Encryption;
using ServiceCore.Encryption.enums;

namespace DataBase.mssqlserver
{
    public class ConnectionInfo : IConnectionInfo
    {
        private const string pattern = "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};";

        public string Host { get; private set; }
        public string Database { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }

        public ConnectionInfo(string host, string database, string user, string password)
        {
            if (string.IsNullOrEmpty(host)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Host"));
            if (string.IsNullOrEmpty(database)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Database"));
            if (string.IsNullOrEmpty(user)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.User"));
            if (string.IsNullOrEmpty(password)) throw new Exception(string.Format(GeneralMessages.ERR_IS_NULL_OR_EMPTY, "ConnectionInfo.Password"));

            Host = DecryptString(host.Trim());
            Database = DecryptString(database.Trim());
            User = DecryptString(user.Trim());
            Password = DecryptString(password.Trim());
        }

        private string DecryptString(string str)
        {
            string clear = str;
            if (!string.IsNullOrEmpty(str))
            {
                IEncryptionAccessor accessor = null;
                //AES
                try
                {
                    accessor = EncryptionAccessor.Create(EncryptionType.AES);
                    clear = accessor.Decrypt(str);
                    return clear;
                }
                catch { }

                //RSA
                try
                {
                    accessor = EncryptionAccessor.Create(EncryptionType.RSA);
                    clear = accessor.Decrypt(str);
                    return clear;
                }
                catch { }
            }
            
            return clear;
        }

        #region IConnectionInfo Members

        public string DBConString
        {
            get { return string.Format(pattern, Host, Database, User, Password); }
        }

        #endregion
    }
}
