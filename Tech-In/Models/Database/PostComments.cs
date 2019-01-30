using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class PostComments
    {
        [Key]
        public int Id { get; set; }
        public string CommentMsg { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
    }
}
