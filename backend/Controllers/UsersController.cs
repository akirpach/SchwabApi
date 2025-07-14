using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace SampleStartUp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult GetUsers()
        {
            var users = _context.Users.ToArray(); 

            return Ok(users);
        }        
    }
}