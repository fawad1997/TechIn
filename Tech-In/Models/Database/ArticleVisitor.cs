using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class ArticleVisitor
    {
        [Key]
        public int Id { get; set; }
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article ArticleRef { get; set; }
        [StringLength(maximumLength: 300)]
        public string UserId { get; set; }
        public Boolean IsLoggedIn { get; set; }
        [StringLength(maximumLength: 100)]
        public string UserIp { get; set; }
    }
}
