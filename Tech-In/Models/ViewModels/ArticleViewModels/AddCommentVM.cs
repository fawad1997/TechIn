using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class AddCommentVM
    {
        public int ArticleId { get; set; }
        [StringLength(maximumLength: 300, MinimumLength = 3), Required]
        public string Comment { get; set; }
    }
}
