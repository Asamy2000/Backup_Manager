using Microsoft.Data.SqlClient;

namespace Db.Bak.Services
{
    public class DbService : IDbService
    {
        private readonly string _connectionString;

        public DbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task BackupDatabaseAsync(string databaseName, string backupPath)
        {
            var query = $"BACKUP DATABASE [{databaseName}] TO DISK='{backupPath}'";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RestoreDatabaseAsync(string databaseName, string backupPath)
        {
            var mdfPath = $@"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\{databaseName}.mdf";
            var ldfPath = $@"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\{databaseName}_log.ldf";

            var query = $@"
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE [{databaseName}] FROM DISK='{backupPath}'
                WITH MOVE '{databaseName}' TO '{mdfPath}',
                     MOVE '{databaseName}_log' TO '{ldfPath}', REPLACE;
                ALTER DATABASE [{databaseName}] SET MULTI_USER;";

            var masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            }.ConnectionString;

            using (var connection = new SqlConnection(masterConnectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }

}
