using System.Data.SqlClient;

namespace Stocks.Data.Nh.Test
{
    public static class UnitTestHelper
    {
        const string MasterConnectionStr = @"server=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";

        public static void DropDatabase(string serverMasterConnectionStr, string dbName)
        {

            var querryDbExistsCommand = $@"if db_id('{dbName}') is not null select 1 else select 0";
            var dropDatabaseCommand = $@"DECLARE @kill varchar(8000); SET @kill = '';  
SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';'  
FROM master..sysprocesses  
WHERE dbid = db_id('{dbName}')

EXEC(@kill);
DROP DATABASE {dbName}";

            using (var connection = new SqlConnection(serverMasterConnectionStr))
            using (var dbExistsCmd = new SqlCommand(querryDbExistsCommand, connection))
            using (var dropDbCmd = new SqlCommand(dropDatabaseCommand, connection))
            {
                connection.Open();
                var dbExists = (int)dbExistsCmd.ExecuteScalar();
                if (dbExists == 1) { dropDbCmd.ExecuteNonQuery(); }
            }
        }

        public static void CreateDatabase(string serverMasterConnectionStr, string dbName)
        {
            var querryDbExistsCommand = $@"if db_id('{dbName}') is not null select 1 else select 0";
            var createDatabaseCommand = $@"CREATE DATABASE {dbName}";

            using (var connection = new SqlConnection(serverMasterConnectionStr))
            using (var dbExistsCmd = new SqlCommand(querryDbExistsCommand, connection))
            using (var createDbCmd = new SqlCommand(createDatabaseCommand, connection))
            {
                connection.Open();
                var dbExists = (int)dbExistsCmd.ExecuteScalar();
                if (dbExists == 0) { createDbCmd.ExecuteNonQuery(); }
            }
        }

        public static void RecreateDatabase(string serverMasterConnectionStr, string dbName)
        {
            DropDatabase(serverMasterConnectionStr, dbName);
            CreateDatabase(serverMasterConnectionStr, dbName);
        }

        public static void RecreateLocalDatabase(string dbName)
        {
            DropDatabase(MasterConnectionStr, dbName);
            CreateDatabase(MasterConnectionStr, dbName);
        }

    }
}
