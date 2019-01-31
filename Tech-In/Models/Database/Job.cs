using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3), Required]
        public string Title { get; set; }
        [Required]
        public int vacancies { get; set; }
        [Required]
        public string Description { get; set; }
        public int MinSalary { get; set; }
        public int MaxSalary { get; set; }
        [Required]
        public JobStatus Status { get; set; }
        [Required]
        public int MinExpereince { get; set; }
        [Required]
        public int MaxExpereince { get; set; }
        [Required]
        public Qualification Qualification { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostDate { get; set; }
        [Required]
        public JobType JobType { get; set; }
        [Required]
        public JobShift JobShift { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int Location { get; set; }
        [ForeignKey("Location")]
        public virtual City City { get; set; }
        public int? PostedBy { get; set; }
        [ForeignKey("PostedBy")]
        public virtual Company Company { get; set; }
    }
    public enum JobStatus
    {
        Active, InActive
    }
    public enum Qualification
    {
        Matriculation, HighSchool, Graduate, Masters, Doctorate, PostDoc
    }
    public enum JobType
    {
        PartTime, FullTime, Intern
    }
    public enum JobShift
    {
        Night, Evening, Morning
    }
}
