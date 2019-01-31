using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class Applicant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //public int Id { get; set; }
        public string AppliedBy { get; set; }
        [ForeignKey("AppliedBy")]
        public virtual ApplicationUser User { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
    }
}
