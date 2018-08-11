using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class ExperienceVM
    {
        public int UserExperienceId { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3), Required]
        public string Title { get; set; }
        [StringLength(maximumLength: 200, MinimumLength = 10)]
        public string Description { get; set; }
        [Display(Name ="Company Name")]
        [StringLength(maximumLength: 100, MinimumLength = 2), Required]
        public string CompanyName { get; set; }
        public Boolean CurrentWorkCheck { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true, NullDisplayText = "Start date can't be Null")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name ="City")]
        [Required(ErrorMessage ="City Name is required.")]
        public int CityId { get; set; }
        [Display(Name ="Country")]
        [Required(ErrorMessage ="Country Name is required.")]
        public int CountryId { get; set; }

        //ApNetUser
        public string UserId { get; set; }


        public string CountryName { get; set; }
        public string CityName { get; set; }

    }
}
