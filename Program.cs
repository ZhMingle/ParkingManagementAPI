using Microsoft.EntityFrameworkCore;
using ParkingManagementAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ParkingManagementAPI.utils;
using ParkingManagementAPI.Services;
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
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5240",
            ValidAudience = "http://localhost:5240",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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

