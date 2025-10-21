namespace MessageWrapperMediatR.Core.Filters
{
    public class PagingFilter
    {
        public int PageSize { get; set; } = 15;
        public int Page { get; set; } = 1;
    }
}
