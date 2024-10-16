using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementAPI.Data;
using ParkingManagementAPI.Models;
using ParkingManagementAPI.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ParkingManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SystemUserController : ControllerBase
    {
        private readonly SmartParkingContext _context;

        public SystemUserController(SmartParkingContext context)
        {
            _context = context;
        }

        // GET: api/systemuser
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemUser>>> GetSystemUsers()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized("No Authorization header found");
            }

            Console.WriteLine($"Authorization Header33: {authHeader}");
            return await _context.SystemUsers.ToListAsync();
        }

        // GET: api/systemuser/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemUser>> GetSystemUser(int id)
        {
            var systemUser = await _context.SystemUsers.FindAsync(id);

            if (systemUser == null)
            {
                return NotFound();
            }

            return systemUser;
        }

        // POST: api/systemuser
        [HttpPost]
        public async Task<ActionResult<SystemUser>> CreateSystemUser(CreateSystemUserDTO createSystemUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // 1. 检查用户名是否已存在
            var existingUser = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Username == createSystemUser.Username);
            if (existingUser != null)
            {
                // 返回 400 错误，表示用户名已存在
                return BadRequest(new { message = "Username already exists" });
            }

            // 2. 将密码加密后保存
            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(createSystemUser.Password); // 对密码进行哈希加密

            var user = new SystemUser
            {
                Username = createSystemUser.Username,
                Email = createSystemUser.Email,
                PhoneNumber = createSystemUser.PhoneNumber,
                PasswordHash = PasswordHash,
                IsActive = createSystemUser.IsActive,
                Role = createSystemUser.Role,
                CreateAt = DateTime.UtcNow
            };


            _context.SystemUsers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSystemUser), new { id = user.Id }, user);
        }

        // PUT: api/systemuser/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSystemUser(int id, SystemUser updatedSystemUser)
        {
            if (id != updatedSystemUser.Id)
            {
                return BadRequest("User ID mismatch");
            }

            _context.Entry(updatedSystemUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/systemuser/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSystemUser(int id)
        {
            var systemUser = await _context.SystemUsers.FindAsync(id);
            if (systemUser == null)
            {
                return NotFound();
            }

            _context.SystemUsers.Remove(systemUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SystemUserExists(int id)
        {
            return _context.SystemUsers.Any(e => e.Id == id);
        }
    }
}
