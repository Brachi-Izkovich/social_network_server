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
    public class MessageController : ControllerBase
    {
        private readonly IService<MessageDto> service;
        private readonly IOwner owner;
        public MessageController(IService<MessageDto> service, IOwner owner)
        {
            this.service = service;
            this.owner = owner;
        }
        // GET: api/<MessageController>
        [HttpGet]
        public async Task<List<MessageDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<MessageController>/5
        [HttpGet("{id}")]
        public async Task<MessageDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<MessageController>
        [HttpPost]
        [Authorize]
        public async Task<MessageDto> Post([FromForm] MessageDto message)
        {
            return await service.Add(message);
        }

        // להוסיף את הפונקציה ()*&^%$#@!
        // PUT api/<MessageController>/5
        [HttpPut("{messageId}")]
        [Authorize]
        public async Task<IActionResult> Put(int messageId, [FromForm] MessageDto message)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(messageId, userId);

            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid(); // המשתמש לא מורשה לשנות

            await service.Update(messageId, message);
            return Ok();
        }



        // DELETE api/<FeedbackController>/5
        [HttpDelete("{messageId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int messageId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(messageId, userId);

            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid(); // user can't delete

            await service.Delete(messageId);
            return Ok();
        }
    }
}
