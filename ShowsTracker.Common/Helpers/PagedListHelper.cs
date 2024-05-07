using Microsoft.EntityFrameworkCore;
using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Common.Helpers
{
    public static class PagedListHelper
    {
        public static async Task<PagedResponseDto<T>> ToPagedListAsync<T>(this IQueryable<T> data, int pageSize, int pageIndex) where T : class
        {
            int totalCount = await data.CountAsync().ConfigureAwait(false);
            int totalPage = totalCount / pageSize;
            var pagedData = new PagedResponseDto<T>()
            {
                CurrentPage = pageIndex,
                TotalPage = totalPage,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            pagedData.Data = await data.Skip(pageSize * pageIndex).Take(pageSize).ToListAsync().ConfigureAwait(false);
            return pagedData;
        }
    }
}
