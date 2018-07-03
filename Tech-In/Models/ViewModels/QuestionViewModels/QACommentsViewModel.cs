using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.QuestionViewModels
{
    public class QACommentsViewModel
    {
        public int UserQACommentID { get; set; }

        [MaxLength(200), MinLength(10)]
        [Required]
        public string Description { get; set; }

        public Boolean Visibility { get; set; }

        public Boolean IsAnswer { get; set; }
        public Nullable<int> UserQuestionId { get; set; }
        public Nullable<int> UserQAnswerId { get; set; }

        //ApNetUser
        public string UserId { get; set; }
     
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
