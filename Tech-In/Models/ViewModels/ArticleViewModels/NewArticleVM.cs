using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class NewArticleVM
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 150, MinimumLength = 5), Required]
        public string Title { get; set; }
        [Display(Name ="Category :"),Required]
        public int CategoryId { get; set; }
        [Display(Name = "Display Image :"),Required]
        public string ArticleImg { get; set; }
        [Display(Name = "Body :"),Required]
        public string ArticleBody { get; set; }
        [Required]
        public string Tags { get; set; }
        
    }
}
