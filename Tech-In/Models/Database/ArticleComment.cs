using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class ArticleComment
    {
        [Key]
        public int Id { get; set; }
        public int OriginalId { get; set; }
        [StringLength(maximumLength: 8, MinimumLength = 5), Required]
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        [StringLength(maximumLength: 300, MinimumLength = 3), Required]
        public string Comment { get; set; }
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article ArticleRef { get; set; }
        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
