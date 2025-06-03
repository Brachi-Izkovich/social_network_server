using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IService<FeedbackDto> service;
        public FeedbackController(IService<FeedbackDto> service)
        {
            this.service = service;
        }
        // GET: api/<FeedbackController>
        [HttpGet]
        public async Task<List<FeedbackDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<FeedbackController>/5
        [HttpGet("{id}")]
        public async Task<FeedbackDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<FeedbackController>
        [HttpPost]
        public async Task<FeedbackDto> Post([FromForm] FeedbackDto feedback)
        {
            return await service.Add(feedback);
        }

        // PUT api/<FeedbackController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromForm] FeedbackDto feedback)
        {
            await service.Update(id, feedback);
        }

        // DELETE api/<FeedbackController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
