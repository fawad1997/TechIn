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
    }
}
