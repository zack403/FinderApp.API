using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinderApp.API.Helpers
{
    public class PagedList <T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages   { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }


        public PagedList(List<T> items, int count, int pageNumber, int pagesize)
        {
            TotalCount = count;
            PageSize = pagesize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(Count / (double)pagesize);
            this.AddRange(items);
            
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pagesize )
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber -1) * pagesize).Take(pagesize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pagesize);
        }


    }
}