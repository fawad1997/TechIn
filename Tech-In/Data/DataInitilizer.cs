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
            //InitilizaCategory(context);
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
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Algorithms"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Analytics"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Architecture software"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Artificial Intelligence"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Big Data"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Business Relationship Management"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Communication Tools"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Configuration Management"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Cost Control Software"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Databases"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Data Analysis"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Data Mining"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Enterprise Resourse Planning"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Internet of Things"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Information Security"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Information Visualization"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Legacy Systems"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="POS Systems"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Project Management"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Risk Management"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Scientific Computing"},
                new Category {ActiveStatus=true,AddedBy="70234f03-9050-4345-a09b-65cbf3babaac",Title="Virtual Reality"}
            }.ToList();
            foreach (Category cat in categories)
                context.Category.Add(cat);
            context.SaveChanges();
        }
    }
}
