using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class SingleUserVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFriendReqSent { get; set; }
        public bool IsFriendReqRecieved { get; set; }
    }
}
