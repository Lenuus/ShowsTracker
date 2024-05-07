using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Common.Dtos
{
    public class PagedResponseDto<T> where T : class
    {
        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }

        public int PageSize { get; set; }

        public List<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
