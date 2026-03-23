using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TeamTrack.Api.Common;

namespace TeamTrack.Api.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, QueryParams param)
    {
        return query
            .Skip((param.Page - 1) * param.PageSize)
            .Take(param.PageSize);
    }

    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string? search,
        Expression<Func<T, string>> property)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        return query.Where(x => property.Compile().Invoke(x).Contains(search));
    }

    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        return sortBy.ToLower() switch
        {
            "name" => query.OrderBy(x => EF.Property<object>(x!, "Name")),
            "name_desc" => query.OrderByDescending(x => EF.Property<object>(x!, "Name")),
            _ => query
        };
    }
}