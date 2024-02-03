using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReactionController(ILogger<ReactionController> logger, IReactionService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string Urn)
        {
            var response = await service.GetReactions(Urn);

            return Ok(response);
        }

    }
}
