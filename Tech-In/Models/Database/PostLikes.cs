using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class PostLikes
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual UserPost PostRef { get; set; }
        [StringLength(maximumLength: 300)]
        public string UserId { get; set; }
    }
}
