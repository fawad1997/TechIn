using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public int OriginalId { get; set; }
        [StringLength(maximumLength: 8, MinimumLength = 5), Required]
        public string Status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateTime { get; set; }
        [StringLength(maximumLength: 150, MinimumLength = 5), Required]
        public string Title { get; set; }
        public string ArticleImg { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string ArticleBody { get; set; }
        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
