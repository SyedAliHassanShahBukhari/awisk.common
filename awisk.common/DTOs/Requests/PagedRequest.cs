namespace awisk.common.DTOs.Requests
{
    /// <summary>
    /// Represents a paged request with pagination parameters.
    /// </summary>
    public class PagedRequest
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        /// <summary>
        /// The page number to retrieve (1-based). Default is 1.
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// The number of items per page. Default is 10. Maximum is 1000.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : (value > 1000 ? 1000 : value);
        }

        /// <summary>
        /// Optional field name to sort by.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Indicates whether to sort in descending order. Default is false (ascending).
        /// </summary>
        public bool SortDescending { get; set; } = false;

        /// <summary>
        /// Gets the zero-based index for database queries (PageNumber - 1).
        /// </summary>
        public int Skip => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Gets the number of items to take (same as PageSize).
        /// </summary>
        public int Take => PageSize;
    }
}

