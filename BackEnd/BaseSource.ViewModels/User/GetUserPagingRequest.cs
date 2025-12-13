using BaseSource.ViewModels.Common;

namespace BaseSource.ViewModels.User
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
