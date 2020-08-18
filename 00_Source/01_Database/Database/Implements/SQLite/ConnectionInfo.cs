using Database.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace Database.Implements.SQLite
{
    public class ConnectionInfo : IConnectionInfo
    {
        private string[] FILE_EXTENSIONS = new string[] { ".db", ".s2db", ".s3db" };

        public string DBFile { get; private set; }
        public string Password { get; private set; }

        public ConnectionInfo(string dbfile, string password = null)
        {
            if (string.IsNullOrWhiteSpace(dbfile)) throw new ArgumentNullException("dbfile");
            var file = dbfile.Trim();

            if (!FILE_EXTENSIONS.Any(e => e.Equals(Path.GetExtension(file).ToLower()))) throw new ArgumentException(string.Format("It`s not a sqlite dbfile({0}).", file), "dbfile");
            if (!Directory.Exists(Path.GetPathRoot(file))) throw new DirectoryNotFoundException(string.Format("Path({0}) does not exist.", Path.GetPathRoot(file)));
            DBFile = new FileInfo(file).FullName;

            Password = password;
        }

        #region IConnectionInfo Members

        public string DBConString
        {
            get
            {
                var connstr = new System.Data.SQLite.SQLiteConnectionStringBuilder();
                connstr.DataSource = DBFile;
                if (Password != null) connstr.Password = Password;
                return connstr.ToString();
            }
        }

        #endregion
    }
}
