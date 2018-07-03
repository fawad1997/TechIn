using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.QuestionViewModels
{
    public class QAVotingViewModel
    {
        public int UserQAVotingID { get; set; }
        public int Value { get; set; }
        public Boolean Visibility { get; set; }
        public Boolean IsAnswer { get; set; }
        public Nullable<int> UserQuestionId { get; set; }
        public Nullable<int> UserQAnswerId { get; set; }
        public string userId { set; get; }
    }
}
