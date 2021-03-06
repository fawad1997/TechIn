﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class UserPublication
    {
        [Key]
        public int UserPublicationId { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 5), Required]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublishYear { get; set; }
        [StringLength(maximumLength: 200, MinimumLength = 15)]
        public string Description { get; set; }
        public Boolean ConferenceOrJournal { get; set; }
        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
