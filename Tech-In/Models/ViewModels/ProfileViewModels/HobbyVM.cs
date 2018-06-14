using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class HobbyVM
    {
        public int UserHobbyId { get; set; }
        [StringLength(maximumLength: 20, MinimumLength = 3), Required]
        public string HobbyOrIntrest { get; set; }
        public string UserId { get; set; }
    }
}
