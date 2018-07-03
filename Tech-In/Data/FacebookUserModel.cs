using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Data
{
    public class FacebookUserModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }
        public string link { get; set; }
        public string username { get; set; }
        public int timezone { get; set; }
        public FacebookLocation location { get; set; }
        public Picture picture { get; set; }
    }
}
