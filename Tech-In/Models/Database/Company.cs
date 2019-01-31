using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tech_In.Models.Database
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3), Required]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string About { get; set; }
        [Required]
        public string WebSite { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime FoundedDate { get; set; }
        [Required]
        public string Industry { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int Size { get; set; }
        [Required]
        public string Speciality { get; set; }
        [Required]
        public int Location { get; set; }
        public string Logo { get; set; }
        [ForeignKey("Location")]
        public virtual City City { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
