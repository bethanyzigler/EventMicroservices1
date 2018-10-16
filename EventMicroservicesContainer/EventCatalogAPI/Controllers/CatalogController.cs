using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCatalogAPI.Data;
using EventCatalogAPI.Domain;
using EventCatalogAPI.ViewModels;
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

        public CatalogController(CatalogContext catalogContext, IConfiguration configuration/*IOptionsSnapshot<CatalogSettings> settings*/)
        {
            _catalogContext = catalogContext;
            _configuration = configuration;
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

            var model = new PaginatedEventsViewModel<CatalogEvent>
                (pageIndex, pageSize, totalItems, eventsOnPage);

            return Ok(model);

        }

        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Events(
            string name,
            [FromQuery] int pageSize = 6,
            [FromQuery] int pageIndex = 0
            )
        {
            var totalItems = await _catalogContext.CatalogEvents
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();
            var eventsOnPage = await _catalogContext.CatalogEvents
                .Where(c => c.Name.StartsWith(name))
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            eventsOnPage = ChangeUrlPlaceHolder(eventsOnPage);

            var model = new PaginatedEventsViewModel<CatalogEvent>(pageIndex, pageSize, totalItems, eventsOnPage);
            return Ok(model);

        }

        
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/company/{catalogCompanyId}")]
        public async Task<IActionResult> Events(
            int? catalogTypeId,
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
            var model = new PaginatedEventsViewModel<CatalogEvent>
                (pageIndex, pageSize, totalItems, eventsOnPage);

            return Ok(model);

        }

        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CatalogEvent @event
            )
        {
            var item = new CatalogEvent
            {
                CatalogCompanyId = @event.CatalogCompanyId,
                CatalogTypeId = @event.CatalogTypeId,
                Description = @event.Description,
                Name = @event.Name,
                PictureUrl = @event.PictureUrl,
                Price = @event.Price
            };
            _catalogContext.CatalogEvents.Add(item);
            await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id });
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
                    _configuration["ExternalCatalogBaseUrl"]);
                return Ok(@event);
            }
            return NotFound();
        }       

        private List<CatalogEvent> ChangeUrlPlaceHolder(List<CatalogEvent> events)
        {
            events.ForEach(
                x => x.PictureUrl =
                x.PictureUrl
                .Replace("http://externalcatalogbaseurltobereplaced",
                _configuration["ExternalCatalogBaseUrl"]));
            return events;
        }

    }
}