using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinkedInController(ILogger<LinkedInController> logger, ILinkedInService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Add(Post post)
        {
            if (await service.CreatePost(post))
                return Ok();
            return Conflict();
        }
    }
}
