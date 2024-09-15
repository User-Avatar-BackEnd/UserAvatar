using System.Collections.Generic;

namespace UserAvatar.Api.Contracts.ViewModels;

/// <summary>
///     List of users paged ViewModel
/// </summary>
public sealed class PagedUserVm
{
    /// <summary>
    ///     Page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    ///     Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    ///     Total pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    ///     Total elements
    /// </summary>
    public int TotalElements { get; set; }

    /// <summary>
    ///     Is this is first page flag
    /// </summary>
    public bool IsFirstPage { get; set; }

    /// <summary>
    ///     Is this is last page flag
    /// </summary>
    public bool IsLastPage { get; set; }

    /// <summary>
    ///     List of users
    /// </summary>
    public List<UserPageDataVm> Users { get; set; }
}
