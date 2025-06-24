using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IService<TopicDto> service;
        public TopicController(IService<TopicDto> service)
        {
            this.service = service;
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
        public async Task<TopicDto> Post([FromBody] TopicDto topic)
        {
            return await service.Add(topic);
        }

        // להוסיף את הפונקציה ()*&^%$#@!
        // PUT api/<TopicController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] TopicDto topic)
        {
            await service.Update(id, topic);
        }

        // להוסיף את הפונקציה ()*&^%$#@!
        // DELETE api/<TopicController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
