namespace Db.Bak.Models
{
    public class DbBackupModel
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DownloadUrl { get; set; }

        public DbBackupModel(FileInfo fileInfo)
        {
            Name = fileInfo.Name;
            CreatedOn = fileInfo.CreationTime;
        }
    }

}
