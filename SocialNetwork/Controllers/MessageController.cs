using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IService<MessageDto> service;
        public MessageController(IService<MessageDto> service)
        {
            this.service = service;
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
        public async Task<MessageDto> Post([FromBody] MessageDto message)
        {
            return await service.Add(message);
        }

        // PUT api/<MessageController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] MessageDto message)
        {
            await service.Update(id, message);
        }

        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
