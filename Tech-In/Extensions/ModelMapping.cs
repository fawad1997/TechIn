using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Models.Database;
using Tech_In.Models.Model;
using Tech_In.Models.ViewModels.ArticleViewModels;
using Tech_In.Models.ViewModels.ProfileViewModels;

namespace Tech_In.Extensions
{
    public class ModelMapping : AutoMapper.Profile
    {
        public ModelMapping()
        {
            CreateMap<UserEducation, EducationVM>().ReverseMap();
            CreateMap<Article, NewArticleVM>().ReverseMap();
        }
    }
}
