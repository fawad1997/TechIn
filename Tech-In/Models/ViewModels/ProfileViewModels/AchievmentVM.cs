using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class AchievmentVM
    {
        public int UserAchievementId { get; set; }
        [StringLength(maximumLength: 70, MinimumLength = 5), Required]
        public string Description { get; set; }
    }
}
