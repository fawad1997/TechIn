using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Extensions;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class UserListVM
    {
        public PaginatedList<SingleUserVM> User { get; set; }
    }
}
