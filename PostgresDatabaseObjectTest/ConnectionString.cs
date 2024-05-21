using System;
using System.IO;

namespace ThrowException.CSharpLibs.PostgresDatabaseObjectTest
{
    public static class ConnectionString
    {
        private const string FileName = "/home/user/dev/config/development/dbobjlib/connectionstring.txt";

        public static string Get()
        {
            return File.ReadAllText(FileName);
        }
    }
}
