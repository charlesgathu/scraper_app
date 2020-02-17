using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scrapper.Service;

namespace Scraper.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {

        private readonly ILogger<ShowsController> logger;
        private readonly IScraperQueryService queryService;

        public ShowsController(IScraperQueryService queryService, ILogger<ShowsController> logger)
        {
            this.queryService = queryService ?? throw new ArgumentNullException(nameof(IScraperQueryService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(ILogger<ShowsController>));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int index)
        {
            try
            {
                var shows = await queryService.GetShowsAsync(index);
                return Ok(shows);
            }
            catch (Exception excp)
            {
                logger.LogError(excp, $"Unable to query shows for index {index}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Unable to query shows for index {index}");
            }
        }

    }
}