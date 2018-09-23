using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Extensions;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class ArticleListVM
    {
        public PaginatedList<SingleArticleVM> Articles { get; set; }
    }
}
