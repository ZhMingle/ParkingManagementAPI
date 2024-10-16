using Microsoft.EntityFrameworkCore;
using ParkingManagementAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ParkingManagementAPI.utils;
using ParkingManagementAPI.Services;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Loading JWT key
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(5)  // Allow a 5-minute clock skew

        };
        // 添加日志记录事件
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check if the token is in the Authorization header
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                context.Token = token;
                Console.WriteLine($"Token received: {context.Token}");  // 打印接收到的Token
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token validation failed1123: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token successfully validated");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<SmartParkingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<CustomerOrderService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // 允许特定域
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // 允许凭证（JWT、Cookies等）传递
    });
});

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new KebabCaseRoutingConvention());
});
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseCors("CorsPolicy");

app.UseStaticFiles(); // 使静态文件（如图片）可访问
app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

