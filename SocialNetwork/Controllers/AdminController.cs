using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromForm] AdminChangeRoleDto dto)
        {
            var result = await _adminService.ChangeUserRoleAsync(dto);

            if(result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
    }
}
