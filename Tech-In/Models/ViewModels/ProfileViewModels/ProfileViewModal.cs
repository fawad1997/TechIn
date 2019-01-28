using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Models.Model;

namespace Tech_In.Models.ViewModels.ProfileViewModels
{
    public class ProfileViewModal
    {
        public string Nam { get; set; }
        public string UserName { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool AreFriends { get; set; }
        public bool IsFriendReqSent { get; set; }
        public bool IsFriendReqRecieved { get; set; }
        public UserPersonalViewModel UserPersonalVM = new UserPersonalViewModel();

        public IEnumerable<EducationVM> EduVMList { get; set; }
        public IEnumerable<ExperienceVM> ExpVMList { get; set; }
        public IEnumerable<CertificationVM> CertificationVMList { get; set; }
        public IEnumerable<AchievmentVM> AchievVMList { get; set; }
        public IEnumerable<HobbyVM> HobbyVMList { get; set; }
        public IEnumerable<LanguageSkillVM> LanguageSkillVMList { get; set; }
        public IEnumerable<PublicationVM> PublicationVMListJP { get; set; }
        public IEnumerable<PublicationVM> PublicationVMListCP { get; set; }
        public IEnumerable<UserPostVM> UserPosts { get; set; }
    }
}
