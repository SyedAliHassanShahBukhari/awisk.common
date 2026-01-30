namespace awisk.common.DTOs.Responses
{
    /// <summary>
    /// Represents a paged response with data and pagination metadata.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// The collection of items for the current page.
        /// </summary>
        public List<T> Data { get; set; } = new();

        /// <summary>
        /// The current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

        /// <summary>
        /// Indicates whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indicates whether there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Creates an empty paged response.
        /// </summary>
        public PagedResponse()
        {
        }

        /// <summary>
        /// Creates a paged response with data and pagination information.
        /// </summary>
        /// <param name="data">The data for the current page.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total count of items.</param>
        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        {
            Data = data?.ToList() ?? new List<T>();
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Creates a paged response from a PagedRequest.
        /// </summary>
        /// <param name="data">The data for the current page.</param>
        /// <param name="request">The paged request.</param>
        /// <param name="totalCount">The total count of items.</param>
        public PagedResponse(IEnumerable<T> data, DTOs.Requests.PagedRequest request, int totalCount)
            : this(data, request.PageNumber, request.PageSize, totalCount)
        {
        }
    }
}

