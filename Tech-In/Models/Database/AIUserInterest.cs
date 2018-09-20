using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class AIUserInterest
    {
        [Key]
        public int Id { get; set; }
        public int Count { get; set; }
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual SkillTag TagRef { get; set; }
        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}