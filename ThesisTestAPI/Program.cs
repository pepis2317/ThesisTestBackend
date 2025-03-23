using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Services;
using ThesisTestAPI.Validators.User;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost",
            ValidAudience = "http://localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("linggangguliguliguligwacalingganggulingganggu"))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
//// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<ThesisDbContext>(options =>
{
    var conString = configuration.GetConnectionString("SQLServerDB");
    options.UseSqlServer(conString);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataProtection();
builder.Services.AddTransient<UserService>();
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddValidatorsFromAssembly(typeof(UploadPfpValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserEditValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserLoginValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserRegisterValidator).Assembly);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5026); // Allows connections from any IP
});

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
