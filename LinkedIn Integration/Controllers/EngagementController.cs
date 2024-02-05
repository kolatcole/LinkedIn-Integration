using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EngagementController(ILogger<EngagementController> logger, IEntityEngagementService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string Urn)
        {
            var response = await service.GetEngagements(Urn);

            return Ok(response);
        }

    }
}
