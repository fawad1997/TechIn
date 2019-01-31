using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class SavedJob
    {
        [Key]
        public Guid Id { get; set; }
        public String User { get; set; }
        [ForeignKey("User")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int Job { get; set; }
        [ForeignKey("Job")]
        public virtual Job JobId { get; set; }
    }
}
