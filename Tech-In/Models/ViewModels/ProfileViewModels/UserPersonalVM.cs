using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Models.Model;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class UserPersonalViewModel
    {
        public int UserPersonalDetailID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name can only contain aplhabets")]
        public string FirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name can only contain aplhabets")]
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string Email { get; set; }

        [StringLength(maximumLength: 300, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 50)]
        public string Summary { get; set; }
        //AspNetUsers
        public string UserID { get; set; }

        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        [Display(Name ="Date of Birth")]
        public DateTime DOB { get; set; }
        public string PhoneNo { get; set; }

        public Gender Gender { get; set; }
        [Required(ErrorMessage = "City name is required")]
        [Display(Name ="City")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Country name is required")]
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
}
