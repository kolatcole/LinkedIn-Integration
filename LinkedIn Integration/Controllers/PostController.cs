using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class PostController(IPostService service, IOptions<LinkedInOptions> _options) : ControllerBase
    {
        //private readonly UserManager<AppUser> _userManager = userManager;
        //private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly LinkedInOptions _linkedInOptions = _options.Value;

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Post post)
        {
            //var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
            var token = _linkedInOptions.Token;
            var response = await service.CreatePost(post, token);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Post Creation Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));
        }

        [HttpPost("Reshare")]
        public async Task<IActionResult> Reshare(Post post)
        {
            //var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
            var token = _linkedInOptions.Token;
            var response = await service.ResharePost(post, token);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Reshare Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, shareUrn = response.Headers.GetValues("x-restli-id").ToArray()[0] }));
        }

        [HttpPost("Update/{Urn}")]
        public async Task<IActionResult> Update(PostUpdate postEntity, string Urn)
        {
            //var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
            var token = _linkedInOptions.Token;
            var response = await service.UpdatePost(postEntity, Urn, token);
            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new Exception("Update Failed");

            return Ok(Created("", new { statusCode = response.StatusCode, message = "Successfully Updated" }));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string Urn)
        {
            //var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
            var token = _linkedInOptions.Token;
            var response = await service.DeletePost(Urn, token);
            if (response.StatusCode != HttpStatusCode.NoContent)
                return Ok(Created("", new { message = "Post is already deleted" }));

            return Ok(Created("", new { statusCode = response.StatusCode, message = "Post was deleted successfully" }));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string Urn)
        {
            //var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
            var token = _linkedInOptions.Token;
            var response = await service.GetPost(Urn, token);

            return Ok(response);
        }

        //[HttpGet("get_user")]
        //public async Task<IActionResult> GetUser()
        //{
        //    //var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
        //    var token = _linkedInOptions.Token;
        //    var response = await service.GetOwner();

        //    return Ok(response);
        //}

    }
}
