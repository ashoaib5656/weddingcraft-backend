using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;

namespace weddingcraft_be.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResponse<IEnumerable<T>>> ToPagedListAsync<T>(
        this IQueryable<T> source, PaginationFilter filter, string message = "Success")
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResponse<IEnumerable<T>>(items, filter.PageNumber, filter.PageSize, count, message);
    }
}
