using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class AddPostVM
    {
        public int Id { get; set; }
        public string PostDescription { get; set; }
        [Display(Name = "Post Picture")]
        public IFormFile PostImg { get; set; }
    }
}
