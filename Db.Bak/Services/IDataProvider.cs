using System.ComponentModel.DataAnnotations;

namespace Db.Bak.Services
{
    public interface IDataProvider
    {
        bool CanBackup { get; }
        bool CanRestore { get; }
        string CreateBackupFileName();
        Task BackupDatabaseAsync(string backupPath);
        Task RestoreDatabaseAsync(string backupPath);
        ValidationResult ValidateBackupFileName(string fileName);
    }

}
