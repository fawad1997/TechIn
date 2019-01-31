using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class PostCommentsView
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
        public DateTime PostTime { get; set; }
    }
}
