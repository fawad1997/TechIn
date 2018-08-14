using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.QuestionViewModels
{
    public class QuestionTagViewModel
    {
        public int SkillTagId { get; set; }
        [Required]
        [MinLength(1),MaxLength(20)]
        public string SkillName { get; set; }

     
    }
}
