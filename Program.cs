using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SellYourStuffWebApi.Data;
using SellYourStuffWebApi.Interfaces;
using SellYourStuffWebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
var token = Environment.GetEnvironmentVariable("Token");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token));

builder.Services.AddDbContext<SellYourStuffWebApiContext>(options =>
{
    options.UseMySQL(connectionString ?? throw new InvalidOperationException("Connection string not found"));
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(
        name: "sellyourstuffClient",
        policy => { policy.WithOrigins("https://sellyourstuff-63568.web.app").AllowAnyMethod().AllowAnyHeader(); }
));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPhotoService, PhotoService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseStaticFiles();
app.UseRouting();
app.UseCors("sellyourstuffClient");

app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello and welcome to Sellyourstuff-api. Please visit https://sellyourstuff-63568.web.app to enjoy the full app. See you there !");
app.Run();
