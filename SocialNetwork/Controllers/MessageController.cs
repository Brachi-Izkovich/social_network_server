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
        [Authorize]
        public async Task<MessageDto> Post([FromForm] MessageDto message)
        {
            return await service.Add(message);
        }

        // PUT api/<MessageController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task Put(int id, [FromBody] MessageDto message)
        {
            //צריך לתת הרשאה רק למנהל או למי שכתב את ההודעה בעזרת הפונקציה שמיורקת מתחת 
            //var userId = User.FindFirst("sub")?.Value;// או ClaimTypes.NameIdentifier
            await service.Update(id, message);
        }


        //[HttpPut("{id}")]
        //[Authorize] // שימי לב – לא Roles, כדי לאפשר גם ליוצר ההודעה
        //public async Task<IActionResult> Put(int id, [FromBody] MessageDto message)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (userId == null)
        //        return Forbid();

        //    var isAdmin = User.IsInRole("Admin");
        //    var messageOwnerId = await service.GetMessageOwnerId(id);

        //    if (userId != messageOwnerId && !isAdmin)
        //        return Forbid();

        //    await service.Update(id, message);
        //    return NoContent();
        //}

        //כנל כמו פוט בהרשאות
        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
