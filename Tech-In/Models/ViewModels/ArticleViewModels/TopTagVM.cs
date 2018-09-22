using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class TopTagVM
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public int Count { get; set; }
    }
}
