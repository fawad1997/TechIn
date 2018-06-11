using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class LanguageSkillVM
    {
        public int LanguageSkillId { get; set; }
        [StringLength(maximumLength: 20, MinimumLength = 3), Required]
        public string SkillName { get; set; }
        //ApNetUser
        public string UserId { get; set; }
    }
}
