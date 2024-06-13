namespace Db.Bak.Models
{
    public class GridModel<T>
    {
        public List<T> Rows { get; set; }
        public int Total { get; set; }
    }

}
