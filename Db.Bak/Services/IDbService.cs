

namespace Db.Bak.Services
{
    public interface IDbService
    {
        Task BackupDatabaseAsync(string databaseName, string backupPath);
        Task RestoreDatabaseAsync(string databaseName, string backupPath);
    }

}
