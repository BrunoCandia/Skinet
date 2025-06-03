namespace Core.Specifications
{
    public class ProductSpecParams : PagingParams
    {
        #region Filtering
        private List<string> _brands = new List<string>();

        public List<string> Brands
        {
            get => _brands; // query string => /api/products?brands=Angular,React&types=Boots,Gloves
            set
            {
                _brands = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
                               .ToList();
            }
        }

        private List<string> _types = new List<string>();

        public List<string> Types
        {
            get => _types; // query string => /api/products?brands=Angular,React&types=Boots,Gloves
            set
            {
                _types = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
                               .ToList();
            }
        }
        #endregion

        #region Sorting
        public string? Sort { get; set; }
        #endregion

        #region Paging
        ////private const int MaxPageSize = 20;

        ////public int PageIndex { get; set; } = 1;

        ////private int _pageSize = 6;

        ////public int PageSize
        ////{
        ////    get => _pageSize;
        ////    set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        ////}
        #endregion

        #region Serching
        private string? _search;

        public string Search
        {
            get => _search ?? string.Empty;
            set => _search = value.ToLower();
        }
        #endregion
    }
}
