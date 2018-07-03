using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Models.Model;

namespace Tech_In.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "First Name ")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name can only contain aplhabets")]
        public string FirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name can only contain aplhabets")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true, NullDisplayText = "Date of Birth can't be Null")]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Display(Name = "Date of Birth Visibility")]
        public Boolean DOBVisibility { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public int CityId { get; set; }

        public int CountryId { get; set; }
        public string Identifier { get; set; }
        public string Picture { get; set; }
    }
}
