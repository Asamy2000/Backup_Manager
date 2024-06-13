namespace Db.Bak.Models
{
    public class GridCommand
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public string Filter { get; set; }
    }
}
