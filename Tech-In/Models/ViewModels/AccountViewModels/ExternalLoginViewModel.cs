using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        //public string Country { get; set; }
        //public string Gender { get; set; }
        public string Identifier { get; set; }
        public string Picture { get; set; }
    }
}
