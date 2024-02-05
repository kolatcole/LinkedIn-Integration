using LinkedIn_Integration.Entities;
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
        [HttpPost]
        public async Task<IActionResult> Create(Comment comment, string postUrn)
        {
            var response = await service.CreateComment(comment, postUrn);
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new Exception("Unpermitted fields present in REQUEST_BODY");
            else if((response.StatusCode == HttpStatusCode.TooManyRequests))
                throw new Exception("Comment create throttled: creation rate limit exceeded for member");

            var commentResponse = JsonSerializer.Deserialize<CommentResponse>(await response.Content.ReadAsStringAsync());
            return Ok(Created("", new { message = $"Comment has been created successfully for post: {postUrn} with id : {commentResponse.Id}", shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));            
        }

        [HttpGet]
        public async Task<IActionResult> Get(string Urn)
        {
            var response = await service.GetComments(Urn);

            return Ok(response);
        }

    }
}
