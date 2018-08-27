using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.QuestionViewModels
{
    public class NewQuestionVM
    {
        
        public int UserQuestionId { get; set; }

        [MinLength(5)]
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(20)]
        public string Description { get; set; }

        public string PostedBy { set; get; }
        public string UserPic { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostTime { get; set; }
        public bool HasVerifiedAns { get; set; }
        public int Visitors { get; set; }
        public int AnswersCount { get; set; }
        [Required]
        public List<QuestionTagViewModel> Tags { get; set; }

        public List<QAnswerViewModel> Answers { get; set; }
        public List<QACommentsViewModel> Comment { get; set; }
        public int Voting { set; get; }
    }
}
