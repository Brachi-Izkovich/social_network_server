using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Interfaces;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IService<CategoryDto> service;

        public CategoryController(IService<CategoryDto> service)
        {
            this.service = service;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        //[AllowAnonymous]
        public async Task<List<CategoryDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<CategoryDto> Get(int id)
        {
            return await service.GetById(id);
        }
        //[HttpGet("getBy/{name}")]
        //public CategoryDto GetCategoryByName(string name)
        //{
        //    return service.get
        //}

        // POST api/<CategoryController>
        [HttpPost]
        [Authorize(Roles = "Manager, Veteran")]
        public Task<CategoryDto> Post([FromForm] CategoryDto category)
        {
            return service.Add(category);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task Put(int id, [FromBody] CategoryDto category)
        {
            await service.Update(id, category);
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
