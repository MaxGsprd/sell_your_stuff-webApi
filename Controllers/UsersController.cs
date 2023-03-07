using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SellYourStuffWebApi.Data;
using SellYourStuffWebApi.Models.Dtos;
using SellYourStuffWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SellYourStuffWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly SellYourStuffWebApiContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(SellYourStuffWebApiContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUser()
        {
            var users = await _context.User.ToListAsync();
            return Ok(users.Select(user => _mapper.Map<UserResponseDto>(user)));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTo = _mapper.Map<UserResponseDto>(user);
            return Ok(userDTo);
        }

        // GET: api/Users/5
        [HttpGet("fullUser/{id}")]
        public async Task<ActionResult<User>> GetFullUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTo = user;
            return Ok(userDTo);
        }

        // PUT: api/Users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutUser(int id, UserRequestDto userDTO)
        {
            if (id != userDTO.Id)
            {
                return BadRequest("id not matching");
            }
            var user = _mapper.Map<User>(userDTO);
            CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        //PATCH: api/Users/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateUser(int id, JsonPatchDocument<User> updatedUser)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                updatedUser.ApplyTo(user);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        // POST: api/Users
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<UserResponseDto>> PostUser(UserRequestDto newUser)
        {
            var isUserNameAlreadyExists = _context.User.Any(x => x.Name == newUser.Name);
            if (isUserNameAlreadyExists)
            {
                return BadRequest("Username already exists");
            }
            var user = _mapper.Map<User>(newUser);
            CreatePasswordHash(newUser.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        //POST api/Users/login
        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login(UserLoginRequestDto loginRequest)
        {
            var user = _context.User.FirstOrDefault(u => u.Name == loginRequest.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Username and password do not match");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpGet("loggedIn")]
        public string GetLoggedUserId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            }
            return result;
        }
    }
}
