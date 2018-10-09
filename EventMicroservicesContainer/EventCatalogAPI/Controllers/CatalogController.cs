using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCatalogAPI.Data;
using EventCatalogAPI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EventCatalogAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Catalog")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _catalogContext;
        private readonly IConfiguration _configuration;
        //private readonly IOptionsSnapshot<CatalogSettings> _settings;

        public CatalogController(CatalogContext catalogContext, IConfiguration configuration/*IOptionsSnapshot<CatalogSettings> settings*/)
        {
            _catalogContext = catalogContext;
            _configuration = configuration;
            //_settings = settings;

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var events = await _catalogContext.CatalogTypes.ToListAsync();
            return Ok(events);

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogCompanies()
        {
            var events = await _catalogContext.CatalogCompanies.ToListAsync();
            return Ok(events);
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Events(
            [FromQuery] int pageSize = 6,
            [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _catalogContext.CatalogEvents
                              .LongCountAsync();
            var eventsOnPage = await _catalogContext.CatalogEvents
                              .OrderBy(c => c.Name)
                              .Skip(pageSize * pageIndex)
                              .Take(pageSize)
                              .ToListAsync();
            eventsOnPage = ChangeUrlPlaceHolder(eventsOnPage);

            //var model = new PaginatedItemsViewModel<CatalogEvent>
            //    (pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(eventsOnPage /*model*/);

        }

        // GET api/Catalog/Items/type/1/brand/null[?pageSize=4&pageIndex=0]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/company/{catalogCompanyId}")]
        public async Task<IActionResult> Events(int? catalogTypeId,
            int? catalogCompanyId,
            [FromQuery] int pageSize = 6,
            [FromQuery] int pageIndex = 0)
        {
            var root = (IQueryable<CatalogEvent>)_catalogContext.CatalogEvents;

            if (catalogTypeId.HasValue)
            {
                root = root.Where(c => c.CatalogTypeId == catalogTypeId);
            }
            if (catalogCompanyId.HasValue)
            {
                root = root.Where(c => c.CatalogCompanyId == catalogCompanyId);
            }

            var totalItems = await root
                              .LongCountAsync();
            var eventsOnPage = await root

                              .OrderBy(c => c.Name)
                              .Skip(pageSize * pageIndex)
                              .Take(pageSize)
                              .ToListAsync();
            eventsOnPage = ChangeUrlPlaceHolder(eventsOnPage);
            //var model = new PaginatedItemsViewModel<CatalogEvent>(pageIndex, pageSize, totalItems, eventsOnPage);

            return Ok(eventsOnPage/*model*/);

        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var @event = await _catalogContext.CatalogEvents
                .SingleOrDefaultAsync(c => c.Id == id);
            if (@event != null)
            {
                @event.PictureUrl = @event.PictureUrl
                    .Replace("http://externalcatalogbaseurltobereplaced",
                    _configuration/*_settings.Value.*/["ExternalCatalogBaseUrl"]);
                return Ok(@event);
            }
            return NotFound();
        }


        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CatalogEvent product)
        {
            var item = new CatalogEvent
            {
                CatalogCompanyId = product.CatalogCompanyId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                //PictureFileName = product.PictureFileName,
                Price = product.Price
            };
            _catalogContext.CatalogEvents.Add(item);
            await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id });
        }


        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateProduct(
            [FromBody] CatalogEvent productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogEvents
                              .SingleOrDefaultAsync
                              (i => i.Id == productToUpdate.Id);
            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }
            catalogItem = productToUpdate;
            _catalogContext.CatalogEvents.Update(catalogItem);
            await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemById), new { id = productToUpdate.Id });
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var product = await _catalogContext.CatalogEvents
                .SingleOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();

            }
            _catalogContext.CatalogEvents.Remove(product);
            await _catalogContext.SaveChangesAsync();
            return NoContent();

        }


        private List<CatalogEvent> ChangeUrlPlaceHolder(List<CatalogEvent> events)
        {
            events.ForEach(
                x => x.PictureUrl =
                x.PictureUrl
                .Replace("http://externalcatalogbaseurltobereplaced",
                _configuration/*_settings.Value.*/["ExternalCatalogBaseUrl"]));
            return events;
        }

    }
}