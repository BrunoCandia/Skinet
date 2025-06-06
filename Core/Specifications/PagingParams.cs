﻿namespace Core.Specifications
{
    public class PagingParams
    {
        #region Paging
        private const int MaxPageSize = 20;

        public int PageIndex { get; set; } = 1;

        private int _pageSize = 6;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        #endregion
    }
}
