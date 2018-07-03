using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class UserQAComment
    {
        [Key]
        public int UserQACommentID { get; set; }

        [StringLength(maximumLength: 200, MinimumLength = 10), Required]
        public string Description { get; set; }

        public Boolean Visibility { get; set; }

        public Boolean IsAnswer { get; set; }
        public Nullable<int> UserQuestionId { get; set; }
        public Nullable<int> UserQAnswerId { get; set; }

        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
