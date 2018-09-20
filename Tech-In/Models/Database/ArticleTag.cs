using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class ArticleTag
    {
        [Key]
        public int Id { get; set; }
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual SkillTag TagRef { get; set; }
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article ArticleRef { get; set; }
    }
}
