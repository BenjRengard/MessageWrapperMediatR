using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Domain.Filters
{
    public class PagingFilter
    {
        public int PageSize { get; set; } = 15;
        public int Page { get; set; } = 1;
    }
}
