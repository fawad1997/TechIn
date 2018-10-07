using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class UserQuestion
    {
        [Key]
        public int UserQuestionId { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3), Required]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostTime { get; set; }
        public Boolean HasVerifiedAns { get; set; }

        [Required]
        public ICollection<QuestionSkill> Tag { set; get; }

        public ICollection<UserQAnswer> UserQAnswer { get; set; }

        public ICollection<UserQAComment> UserQAComment { get; set; }

        public ICollection<UserQAVoting> UserQAVoting { get; set; }

        public ICollection<QuestionVisitor> QuestionVisitor { get; set; }

        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
