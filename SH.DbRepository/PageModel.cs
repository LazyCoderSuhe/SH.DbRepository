namespace SH.DbRepository
{
    public interface IPage<T>
    {
        public IEnumerable<T> Datas { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PageModel<T> : IPage<T>
    {
        public IEnumerable<T> Datas { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
