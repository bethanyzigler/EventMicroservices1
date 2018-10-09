using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventCatalogAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Pic")]
    public class PicController : Controller
    {
        private readonly IHostingEnvironment _env;
        public PicController(IHostingEnvironment env) //provides environment if deployed on another platform
        {
            _env = env;
        }

        [HttpGet]
        [Route("{id}")] //grabs ID from picture - appended to the end of the path
        public IActionResult GetImage(int id) //second parameter must match the route name
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot + "/pics/", "events-" + id + ".jpg");
            var buffer = System.IO.File.ReadAllBytes(path);
            return File(buffer, "image/jpg");
        }
    }
}