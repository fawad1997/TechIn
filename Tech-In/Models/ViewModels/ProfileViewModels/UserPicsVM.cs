using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class UserPicsVM
    {
        public int Id { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }
        [Display(Name = "Cover Picture")]
        public IFormFile CoverImage { get; set; }
        public string PPic { get; set; }
        public string Cpic { get; set; }
        [StringLength(maximumLength: 300, MinimumLength = 20)]
        public string Summary { get; set; }
    }
}