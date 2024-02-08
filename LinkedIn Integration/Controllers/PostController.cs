using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController(IPostService service) : ControllerBase
    {
        [HttpPost("Create/{token}")]
        public async Task<IActionResult> Create(Post post, string token)
        {
            var response = await service.CreatePost(post, token);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Post Creation Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));            
        }
        [HttpPost("Reshare/{token}")]
        public async Task<IActionResult> Reshare(Post post, string token)
        {
            var response = await service.ResharePost(post, token);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Reshare Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));
        }
        [HttpPost("Update/{Urn}/{token}")]
        public async Task<IActionResult> Update(PostUpdate postEntity,string Urn, string token)
        {
            var response = await service.UpdatePost(postEntity, Urn, token);
            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new Exception("Update Failed");
            
            return Ok(Created("", new { statusCode = response.StatusCode, message = "Successfully Updated" }));
        }
        [HttpDelete("{token}")]
        public async Task<IActionResult> Delete(string Urn, string token)
        {
            var response = await service.DeletePost(Urn, token);
            if (response.StatusCode != HttpStatusCode.NoContent)
                return Ok(Created("", new {message = "Post is already deleted" }));

            return Ok(Created("", new { statusCode = response.StatusCode, message = "Post was deleted successfully" }));
        }
        [HttpGet("{token}")]
        public async Task<IActionResult> Get(string Urn, string token)
        {
            var response = await service.GetPost(Urn, token);

            return Ok(response);
        }


    }
}
