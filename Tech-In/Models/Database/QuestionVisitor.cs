using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class QuestionVisitor
    {
        public int QuestionVisitorId { get; set; }
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual UserQuestion Question { get; set; }
        public string UserId { get; set; }
        public Boolean IsLoggedIn { get; set; }
        public string UserIp { get; set; }
    }
}
