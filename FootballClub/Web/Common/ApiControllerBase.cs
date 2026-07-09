using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Common;

[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected static (int Page, int PageSize) NormalizePaging(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
        return (normalizedPage, normalizedPageSize);
    }

    protected static PagedResultDto<T> CreatePagedResult<T>(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        return new PagedResultDto<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}