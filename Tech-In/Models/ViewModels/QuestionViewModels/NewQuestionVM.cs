using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.QuestionViewModels
{
    public class NewQuestionVM
    {
        
        public int UserQuestionID { get; set; }

        [MinLength(3)]
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(10)]
        public string Description { get; set; }

        public string PostedBy { set; get; }

        [Required]
        public List<QuestionTagViewModel> Tags { get; set; }

        public List<QAnswerViewModel> Answers { get; set; }
        public List<QACommentsViewModel> Comment { get; set; }
        public int Voting { set; get; }
    }
}
