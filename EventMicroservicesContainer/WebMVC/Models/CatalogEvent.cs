using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvc.Models
{
    public class CatalogEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public int CatalogCompanyId { get; set; }
        public string CatalogCompany { get; set; }
        public int CatalogTypeId { get; set; }
        public string CatalogType { get; set; }

    }
}
