using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class UserPostVM
    {
        public int UserPostId { get; set; }
        public int OriginalId { get; set; }
        [StringLength(maximumLength: 8, MinimumLength = 5), Required]
        public string Status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateTime { get; set; }

        [StringLength(maximumLength: 1000, MinimumLength = 3)]
        public string Summary { get; set; }

        public string Image { get; set; }

        //ApNetUser
        public string UserId { get; set; }
        public Boolean IsLiked { get; set; }
        public int TotalLikes { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public string GetPastTime()
        {
            TimeSpan t = DateTime.Now - CreateTime;
            string msg = null;
            if (t.Days > 7)
            {
                if (t.Days / 7 == 1)
                    msg = t.Days / 7 + " week ago";
                else
                    msg = t.Days / 7 + " weeks ago";
            }
            else if (t.Days > 0)
            {
                if (t.Days == 1)
                    msg = t.Days + " day ago";
                else
                    msg = t.Days + " days ago";
            }
            else if (t.Hours > 0)
            {
                if (t.Hours == 1)
                    msg = t.Hours + " hour ago";
                else
                    msg = t.Hours + " hours ago";
            }
            else if (t.Minutes > 0)
            {
                if (t.Minutes == 1)
                    msg = t.Minutes + " minute ago";
                else
                    msg = t.Minutes + " minutes ago";
            }
            else
            {
                msg = t.Seconds + " seconds ago";
            }
            return msg;
        }
    }
}
