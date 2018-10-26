using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMvc.Infrastructure;
using WebMvc.Models;

namespace WebMvc.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        public CatalogService(IOptionsSnapshot<AppSettings> settings,
            IHttpClient httpClient)
        {
            _settings = settings;
            _apiClient = httpClient;
            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/catalog/";

        }

        public async Task<IEnumerable<SelectListItem>> GetCompanies()
        {
            var getCompaniesUri = ApiPaths.Catalog.GetAllCompanies(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getCompaniesUri);

            var events = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            var companies = JArray.Parse(dataString);

            foreach (var company in companies.Children<JObject>())
            {
                events.Add(new SelectListItem()
                {
                    Value = company.Value<string>("id"),
                    Text = company.Value<string>("company")
                });
            }

            return events;
        }

        public async Task<Catalog> GetCatalogEvents(int page, int take, int? company, int? type)
        {
            var allcatalogEventsUri = ApiPaths.Catalog.GetAllCatalogEvents(_remoteServiceBaseUrl, page, take, company, type);

            var dataString = await _apiClient.GetStringAsync(allcatalogEventsUri);

            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var getTypesUri = ApiPaths.Catalog.GetAllTypes(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getTypesUri);

            var events = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            var companies = JArray.Parse(dataString);
            foreach (var company in companies.Children<JObject>())
            {
                events.Add(new SelectListItem()
                {
                    Value = company.Value<string>("id"),
                    Text = company.Value<string>("type")
                });
            }
            return events;
        }
    }
}
