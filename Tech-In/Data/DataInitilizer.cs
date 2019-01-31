using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Data;

namespace Tech_In.Models.Database
{
    public static class DataInitilizer
    {
        public static void Initilize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            InitilizaCategory(context);
            //Populating Countries
            if (context.Country.Any())
                return;
            var countries = new Country[]
            {
                new Country {CountryId=1, CountryCode = "AF", CountryName = "Afghanistan", CountryPhoneCode = "+93" },
                new Country {CountryId=2, CountryCode = "AU", CountryName = "Australia", CountryPhoneCode = "+78" },
                new Country {CountryId=3, CountryCode = "AS", CountryName = "Austria", CountryPhoneCode = "+56" },
                new Country{CountryId=4,CountryCode="PK",CountryName="Pakistan",CountryPhoneCode="+92"}
            }.ToList();
            foreach (Country country in countries)
                context.Country.Add(country);
            context.SaveChanges();

            //Initilizing Cities
            if (context.City.Any())
                return;
            var cities = new City[]
            {
                 new City { CityId = 1, CityName = "Kabul", CountryId = 1 },
                 new City { CityId = 2, CityName = "Herat", CountryId = 1 },
                 new City { CityId = 3, CityName = "Jallabad", CountryId = 1 },

                 new City { CityId = 4, CityName = "Sydney", CountryId = 2 },
                 new City { CityId = 5, CityName = "Melbourne", CountryId = 2 },
                 new City { CityId = 6, CityName = "Perth", CountryId = 2 },

                 new City { CityId = 7, CityName = "Vienna", CountryId = 3 },
                 new City { CityId = 8, CityName = "Salzburg", CountryId = 3 },
                 new City { CityId = 9, CityName = "Graz", CountryId = 3 },

                 new City{ CityId = 10, CityName = "Islamabad", CountryId = 4},
                 new City{ CityId = 11, CityName = "Rawalpindi", CountryId = 4},
                 new City{ CityId = 12, CityName = "Lahore", CountryId = 4},
                 new City{ CityId = 13, CityName = "Karachi", CountryId = 4},
                 new City{ CityId = 14, CityName = "Peshawar", CountryId = 4},
                 new City{ CityId = 15, CityName = "Queta", CountryId = 4},
            }.ToList();
            foreach (City city in cities)
                context.City.Add(city);
            context.SaveChanges();
        }
        public static void InitilizaCategory(ApplicationDbContext context)
        {
            if (context.Category.Any())
                return;
            var categories = new Category[]
            {
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Algorithms"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Analytics"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Architecture software"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Artificial Intelligence"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Big Data"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Business Relationship Management"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Communication Tools"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Configuration Management"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Cost Control Software"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Databases"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Data Analysis"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Data Mining"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Enterprise Resourse Planning"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Internet of Things"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Information Security"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Information Visualization"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Legacy Systems"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="POS Systems"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Project Management"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Risk Management"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Scientific Computing"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Virtual Reality"},
                new Category {ActiveStatus=true,AddedBy="19d58371-1426-48ee-a300-b22a3613ef0b",Title="Other"}
            }.ToList();
            foreach (Category cat in categories)
                context.Category.Add(cat);
            context.SaveChanges();

            Article article = new Article
            {
                ArticleBody = "<p>You can easily publish articles on TechIn. Go to the articles nav, click on new article, Write, <strong>Title</strong>, Upload an Cover Imgae of article of minimum height <strong>400px</strong> and minimun width <strong>700px.</strong> Select an category from dropdown list and write description, in the tags section you can select maximum 5 tags.</p>",
                ArticleImg = "/images/article/articlecover290119114822AM.png",
                CreateTime = DateTime.Now,
                Status = "active",
                OriginalId = 1,
                Title = "Publish Articles on TechIn",
                UserId = "19d58371-1426-48ee-a300-b22a3613ef0b"
            };
            context.Article.Add(article);
            context.SaveChanges();
        }
    }
}
