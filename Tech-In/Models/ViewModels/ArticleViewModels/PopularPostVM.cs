using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class PopularPostVM
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleImage { get; set; }
        public DateTime CreateTime { get; set; }
        public int VisitorCount { get; set; }
    }
}
