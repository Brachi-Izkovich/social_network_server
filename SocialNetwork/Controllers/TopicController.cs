using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IService<TopicDto> service; 
        private readonly IOwner owner;
        public TopicController(IService<TopicDto> service, IOwner owner)
        {
            this.service = service;
            this.owner = owner;
        }
        // GET: api/<TopicController>
        [HttpGet]
        public async Task<List<TopicDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<TopicController>/5
        [HttpGet("{id}")]
        public async Task<TopicDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<TopicController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] TopicDto topicDto)
        {
            await service.Add(topicDto);
            return Ok();
        }


        // PUT api/<FeedbackController>/5
        [HttpPut("{topicId}")]
        [Authorize]
        public async Task<IActionResult> Put(int topicId, [FromForm] TopicDto topic)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(topicId, userId);

            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid(); // user can't update

            await service.Update(topicId, topic);
            return Ok();
        }


        // DELETE api/<FeedbackController>/5
        [HttpDelete("{topicId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int topicId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(topicId, userId);

            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid(); // user can't delete

            await service.Delete(topicId);
            return Ok();
        }

        [HttpPost("search-similar")]
        public async Task<List<TopicDto>> SearchSimilar([FromBody] string text)
        {
            var topicService = service as TopicService;
            if (topicService == null)
                throw new Exception("Advanced search only available in TopicService");

            return await topicService.SearchSimilarTopicsAndMessages(text);
        }
    }
}
