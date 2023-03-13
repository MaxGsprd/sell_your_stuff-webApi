using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SellYourStuffWebApi.Data;
using SellYourStuffWebApi.Interfaces;
using SellYourStuffWebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

builder.Services.AddDbContext<SellYourStuffWebApiContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SellYourStuffWebApi") ?? throw new InvalidOperationException("Connection string SellYourStuffWebApiContext not found"));
});
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(
        name: "sellyourstuffClient",
        policy => { policy.WithOrigins("https://sellyourstuffangular.azurewebsites.net").AllowAnyMethod().AllowAnyHeader(); }
));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
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

app.UseStaticFiles();
app.UseCors("sellyourstuffClient");
app.UseRouting();

app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
