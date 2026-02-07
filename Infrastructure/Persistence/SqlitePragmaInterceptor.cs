using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public sealed class SqlitePragmaInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(System.Data.Common.DbConnection connection, ConnectionEndEventData eventData)
        {
            if (connection is SqliteConnection sqlite)
            {
                using var cmd = sqlite.CreateCommand();
                cmd.CommandText = """
PRAGMA foreign_keys = ON;
""";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
