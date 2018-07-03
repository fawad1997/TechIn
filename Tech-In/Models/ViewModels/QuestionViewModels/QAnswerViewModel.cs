using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.QuestionViewModels
{
    public class QAnswerViewModel
    {
        public int UserQAnswerId { get; set; }

        [MinLength(10)]
        [Required]
        public string Description { get; set; }

        public int QuestionId { get; set; }

        public string User { get; set; }
        public string Date { get; set; }

    }
}
