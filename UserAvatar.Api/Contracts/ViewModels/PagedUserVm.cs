using System.Collections.Generic;

namespace UserAvatar.Api.Contracts.ViewModels
{
    public class PagedUserVm
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int TotalElements { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public List<UserPageDataVm> Users { get; set; }
    }
}
