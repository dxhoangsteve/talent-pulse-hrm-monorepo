namespace BaseSource.ViewModels.Common
{
    public class PagingRequestBase
    {
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value; // Limit to 100
        }
    }
}
