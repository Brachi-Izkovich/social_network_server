using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IService<FeedbackDto> service;
        private readonly IOwner owner;
        public FeedbackController(IService<FeedbackDto> service, IOwner owner)
        {
            this.service = service;
            this.owner = owner;
        }
        // GET: api/<FeedbackController>
        
        [HttpGet]
        public async Task<List<FeedbackDto>> Get()
        {
            return await service.GetAll();
        }
        //?????????
        // GET api/<FeedbackController>/5
        [HttpGet("{id}")]
        public async Task<FeedbackDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<FeedbackController>
        [HttpPost]
        [Authorize]
        public async Task<FeedbackDto> Post([FromForm] FeedbackDto feedback)
        {
            return await service.Add(feedback);
        }

        // PUT api/<FeedbackController>/5
        [HttpPut("{id}")]
        [Authorize]
        //public async Task<IActionResult> Put(int id, [FromForm] FeedbackDto feedback)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        //           ?? User.FindFirst("userId")?.Value;

        //    if (userIdClaim == null)
        //        return Unauthorized(); //no correctly token
        //    var userId = int.Parse(userIdClaim);

        //    var isOwner = await owner.IsOwner(id, userId);

        //    if (!isOwner)
        //        return Forbid(); // המשתמש לא מורשה לשנות

        //    await service.Update(id, feedback);
        //    return Ok();
        //}
        public async Task<IActionResult> Put(int id, [FromForm] FeedbackDto feedback)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(id, userId);

            if (!isOwner)
                return Forbid(); // המשתמש לא מורשה לשנות

            await service.Update(id, feedback);
            return Ok();
        }

        // DELETE api/<FeedbackController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles= "Admin")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }


        [HttpGet("whoami")]
        [Authorize]
        public IActionResult WhoAmI()
        {
            var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            return Ok(new { nameClaim, userIdClaim });
        }
    }
}
