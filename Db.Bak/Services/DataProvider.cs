using System.ComponentModel.DataAnnotations;

namespace Db.Bak.Services
{
    public class DataProvider : IDataProvider
    {
        public bool CanBackup => true; 
        public bool CanRestore => true; 

        public string CreateBackupFileName()
        {
            return $"backup_{DateTime.Now:yyyyMMddHHmmss}.bak";
        }

        public Task BackupDatabaseAsync(string backupPath)
        {
            // Implement actual backup logic
            return Task.CompletedTask;
        }

        public Task RestoreDatabaseAsync(string backupPath)
        {
            // Implement actual restore logic
            return Task.CompletedTask;
        }

        public ValidationResult ValidateBackupFileName(string fileName)
        {
            // Implement actual validation logic
            return new ValidationResult { IsValid = true, Version = new Version(1, 0), MatchesCurrentVersion = true };
        }
    }
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public Version Version { get; set; }
        public bool MatchesCurrentVersion { get; set; }
    }


}
