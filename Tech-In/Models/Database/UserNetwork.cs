using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class UserNetwork
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime RecordTime { get; set; }
        public Boolean AreFriend { get; set; }
        //ApNetUser Sender
        public string User1 { get; set; }
        [ForeignKey("User1")]
        public virtual ApplicationUser ApplicationUser1 { get; set; }
        //ApNetUser Reciever
        public string User2 { get; set; }
        [ForeignKey("User2")]
        public virtual ApplicationUser ApplicationUser2 { get; set; }
    }
}
