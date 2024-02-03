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
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Post post)
        {
            var response = await service.CreatePost(post);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Post Creation Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));            
        }
        [HttpPost("Reshare")]
        public async Task<IActionResult> Reshare(Post post)
        {
            var response = await service.ResharePost(post);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Reshare Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));
        }
        [HttpPost("Update/{Urn}")]
        public async Task<IActionResult> Update(Post post)
        {
            var response = await service.UpdatePost(post,"","");
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Reshare Failed");
            
            return Ok(Created("", new { statusCode = response.StatusCode, message = "Successfully Reposted" }));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string Urn)
        {
            var response = await service.DeletePost(Urn);
            if (response.StatusCode != HttpStatusCode.NoContent)
                return Ok(Created("", new {message = "Post is already deleted" }));

            return Ok(Created("", new { statusCode = response.StatusCode, message = "Post was deleted successfully" }));
        }
        [HttpGet]
        public async Task<IActionResult> Get(string Urn)
        {
            var response = await service.GetPost(Urn);

            return Ok(response);
        }


    }
}
