using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpRequests;
using LinkedIn_Integration.HttpEntities.HttpResponses;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController(ICommentService service) : ControllerBase
    {
        [HttpPost("{postUrn}/{token}")]
        public async Task<IActionResult> Create(Comment comment, string postUrn, string token)
        {
            var response = await service.CreateComment(comment, postUrn, token);
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new Exception("Unpermitted fields present in REQUEST_BODY");
            else if((response.StatusCode == HttpStatusCode.TooManyRequests))
                throw new Exception("Comment create throttled: creation rate limit exceeded for member");

            
            var id = response.Headers.GetValues("x-restli-id").ToArray()[0];
            return Ok(Created("", new { message = $"Comment has been created successfully for post: {postUrn} with id : {id}" }));            
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> Get(string Urn, string token)
        {
            var response = await service.GetComments(Urn, token);

            return Ok(response);
        }

    }
}
