using EventCatalogAPI.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventCatalogAPI.Data
{ //dummy data to test application

    public class CatalogSeed
    {
        public static async Task SeedAsync(CatalogContext context)
        {
            context.Database.Migrate();
            if (!context.CatalogCompanies.Any())
            {
                context.CatalogCompanies.AddRange(GetPreconfiguredCatalogCompanies());
                context.SaveChanges(); //commits data to the database

            }
            if (!context.CatalogTypes.Any())
            {
                context.CatalogTypes.AddRange(GetPreconfiguredCatalogTypes());
                context.SaveChanges(); //commits data to the database

            }
            if (!context.CatalogEvents.Any())
            {
                context.CatalogEvents.AddRange(GetPreconfiguredCatalogEvents());
                context.SaveChanges(); //commits data to the database

            }
        }
        private static IEnumerable<CatalogCompany> GetPreconfiguredCatalogCompanies()
        {
            return new List<CatalogCompany>() //dummy list 
            {
                new CatalogCompany {Company = "24 Hour Fitness"},
                new CatalogCompany {Company = "Apple"},
                new CatalogCompany {Company = "sony"}
            };
        }

        private static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>() //dummy list 
            {
                new CatalogType {Type = "Fitness"},
                new CatalogType {Type = "Technology"},
                new CatalogType {Type = "Music"}
            };
        }

        private static IEnumerable<CatalogEvent> GetPreconfiguredCatalogEvents()
        {
            return new List<CatalogEvent>()
            {
                //need new pictureURLs

                new CatalogEvent() { CatalogTypeId=2,CatalogCompanyId=3, Description = "Technology from the future", Name = "Future Tech", Price = 1.00M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/1" },
                new CatalogEvent() { CatalogTypeId=1,CatalogCompanyId=1, Description = "Home exercise strategies", Name = "Get Fit", Price= 0.0M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/2" },
                new CatalogEvent() { CatalogTypeId=2,CatalogCompanyId=2, Description = "Introduction to Iphone X", Name = "Iphone X", Price = 129, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/3" },
                new CatalogEvent() { CatalogTypeId=1,CatalogCompanyId=1, Description = "Techniques for safe running", Name = "Run Safe", Price = 12, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/4" }

            };
        }
    }

}