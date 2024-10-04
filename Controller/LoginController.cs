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


[Route("api/[controller]")]
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

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);  // 这应该是一个更安全的密钥

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "admin")
            }),
            Expires = DateTime.UtcNow.AddHours(1),  // Token 1小时后过期
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
