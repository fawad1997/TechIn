using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class JobSkill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual SkillTag TagRef { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
        public int ActiveJobs { get; set; }
    }
}
