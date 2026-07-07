namespace Catalog.Core.Specs
{
    public class Pagination <T> where T : class
    {
        public Pagination()
        {
            
        }
        public Pagination(int pageSize, int pageIndex,int count , IReadOnlyList<T> data)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Count = count;
            Data = data;
        }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
        public int PageSize { get; set; }
    }
}
