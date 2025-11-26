using Firmeza.Data.Entities;
using Firmeza.Models;
using FirmezaAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FirmezaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<Person> userManager, 
            SignInManager<Person> signInManager, 
            IConfiguration configuration,
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            _logger.LogInformation("Register attempt for {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for {Email}", model.Email);
                return BadRequest(ModelState);
            }

            var user = new Client
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.Name} {model.LastName}",
                Document = model.Document,
                Phone = model.Phone ?? string.Empty,
                RegisterDate = DateTime.UtcNow,
                Address = string.Empty, // Default
                Age = 18 // Default age to avoid 0 if constraint exists, though 0 is valid byte
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created successfully", model.Email);
                // Ensure Client role exists
                if (!await _roleManager.RoleExistsAsync("Client"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>("Client"));
                }

                await _userManager.AddToRoleAsync(user, "Client");
                return Ok(new { Message = "User registered successfully" });
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error creating user {Email}: {Code} - {Description}", model.Email, error.Code, error.Description);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var token = await GenerateJwtToken(user!);
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid login attempt");
        }

        private async Task<string> GenerateJwtToken(Person user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
