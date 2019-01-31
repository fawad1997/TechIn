using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class JobCatagory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category CategoryRef { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job JobRef { get; set; }
        public int ActiveJobs { get; set; }
    }
}
