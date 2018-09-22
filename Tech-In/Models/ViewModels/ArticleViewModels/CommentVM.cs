using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ArticleViewModels
{
    public class CommentVM
    {
        public int Id { get; set; }
        public int OriginalId { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string Comment { get; set; }
        public int ArticleId { get; set; }
        //ApNetUser
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImg { get; set; }

        public string GetPastTime()
        {
            TimeSpan t = DateTime.Now - CreateTime;
            string msg = null;
            if (t.Days > 7)
            {
                if(t.Days/7==1)
                    msg = t.Days / 7 + " week ago";
                else
                    msg = t.Days / 7 + " weeks ago";
            }
            else if (t.Days > 0)
            {
                if(t.Days==1)
                    msg = t.Days + " day ago";
                else
                    msg = t.Days + " days ago";
            }else if (t.Hours > 0)
            {
                if(t.Hours==1)
                    msg = t.Hours + " hour ago";
                else
                    msg = t.Hours + " hours ago";
            }
            else if (t.Minutes > 0)
            {
                if(t.Minutes==1)
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
