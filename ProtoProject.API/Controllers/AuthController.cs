using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using ProtoProject.API.Data;
using ProtoProject.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly DataContext _context;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, DataContext context) 
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        [HttpGet("getId")]
        [Authorize]
        public ActionResult<int> GetUserId()
        {
            var email = User?.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null) 
            {
                return BadRequest($"user with email {email} is not found");
            }
            
            return Ok(user.UserId);
        }

        [HttpPost("cryptPassword/{password}")]
        public ActionResult<string> CryptPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(passwordHash);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(AuthUser request)
        {
            _logger.LogInformation("Register attempt for email: {Email}", request.Email);

            if (_context.Users.FirstOrDefault(u => u.Email == request.Email) != null)
            {
                _logger.LogWarning("User already exists: {Email}", request.Email);
                return BadRequest("user exists");
            }

            // TODO: advanced email verification

            //string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new User
            {
                Name = "User",
                Email = request.Email,
                Password = passwordHash
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Register attempt for email success: {Email}", request.Email);

            return Ok(newUser);
        }


        [HttpPost("login")]
        public ActionResult<User> Login(AuthUser request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || user.Email != request.Email)
            {
                _logger.LogWarning("User not found for email: {Email}", request.Email);
                return BadRequest("user not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                _logger.LogWarning("Incorrect password for email: {Email}", request.Email);
                return BadRequest("password incorrect");
            }

            string token = CreateToken(user);

            //var refreshToken = GenerateRefreshToken();
            //SetRefreshToken(refreshToken);

            _logger.LogInformation("User successfully logged in: {Email}", request.Email);

            return Ok(token);
        }

        //[HttpPost("refresh-token")]
        //public async Task<ActionResult<string>> RefreshToken()
        //{
        //    var refreshToken = Request.Cookies["refreshToken"];

        //    if (!user.RefreshToken.Equals(refreshToken))
        //    {
        //        return Unauthorized("Invalid refresh token");
        //    }
        //    else if (user.TokenExpires < DateTime.Now)
        //    {
        //        return Unauthorized("Token expired");
        //    }

        //    string token = CreateToken(user);
        //    var newRefreshToken = GenerateRefreshToken();
        //    SetRefreshToken(newRefreshToken);

        //    return Ok(token);
        //}

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        //private void SetRefreshToken(RefreshToken newRefreshToken)
        //{
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Expires = newRefreshToken.Expires
        //    };
        //    Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

        //    user.RefreshToken = newRefreshToken.Token;
        //    user.TokenCreated = newRefreshToken.Created;
        //    user.TokenExpires = newRefreshToken.Expires;
        //}

        private string CreateToken(User user)
        {
            _logger.LogInformation("CreateToken attempt for email: {Email}", user.Email);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("CreateToken attempt for email success: {Email}", user.Email);

            return jwt;
        }
    }
}
