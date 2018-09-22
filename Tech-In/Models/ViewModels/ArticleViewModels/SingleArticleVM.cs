using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Models.Database;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class SingleArticleVM
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 150, MinimumLength = 5), Required]
        public string Title { get; set; }
        [Display(Name = "Category :"), Required]
        public int CategoryId { get; set; }
        [Display(Name = "Display Image :"), Required]
        public string ArticleImg { get; set; }
        [Display(Name = "Body :"), Required]
        public string ArticleBody { get; set; }
        public List<SkillTag> Tags { get; set; }
        public DateTime CreateTime { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImg { get; set; }
        public string AuthorSummary { get; set; }
        public int CommentsCount { get; set; }
        public int VisitorsCount { get; set; }
    }
}
