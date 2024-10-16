using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ParkingManagementAPI.Models;
using ParkingManagementAPI.Models.DTO;
using System.Security.Claims;
using System.Text;
using ParkingManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly SmartParkingContext _context;

    public AuthController(IConfiguration config, SmartParkingContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {

        // 1. 验证用户名和密码
        var user = await AuthenticateUser(login.Username, login.Password);

        if (user != null)
        {
            var token = GenerateJwtToken();  // 2. 如果用户存在并且密码正确，生成 JWT 令牌
            return Ok(new { token, user.Username, user.Role });
        }

        return Unauthorized();
    }

    private async Task<SystemUser> AuthenticateUser(string username, string password)
    {
        // 使用数据库查询用户名
        var user = await _context.SystemUsers.FirstOrDefaultAsync(u => u.Username == username);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // 使用BCrypt进行密码验证
        {
            return user;
        }

        return null;
    }


    // 生成 JWT token 的方法
    private string GenerateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));  // Ensure this key is secure and long enough

        // Set up the token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(30),  // Token expires in 30 minutes
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            Issuer = _config["Jwt:Issuer"],  // Optional: set if you validate the issuer
            Audience = _config["Jwt:Audience"]  // Optional: set if you validate the audience
        };

        // Create the token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the token as a string
        return tokenHandler.WriteToken(token);

    }
}
