namespace MessageWrapperMediatR.Domain.Filters
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
    }
}
