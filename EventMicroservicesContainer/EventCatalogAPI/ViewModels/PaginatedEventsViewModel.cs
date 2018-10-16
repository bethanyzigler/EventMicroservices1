using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventCatalogAPI.ViewModels
{
    public class PaginatedEventsViewModel<TEntity>
        where TEntity : class
    {
        public int PageSize { get; private set; }

        public int PageIndex { get; private set; }

        public long Count { get; private set; }

        public IEnumerable<TEntity> Data { get; set; }

        public PaginatedEventsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }

    }
}
