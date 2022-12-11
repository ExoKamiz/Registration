using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Registration.Models;

namespace Registration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrUsersController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        private readonly RegistrationContext _registrationContext;

        public RegistrUsersController(IConfiguration configuration, RegistrationContext registrationContext)
        {
            _configuration = configuration;
            _registrationContext = registrationContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrUser>> Register(UserDTOReg request)
        {
            var currentUser = _registrationContext.RegistrUsers.FirstOrDefault(x => x.UserNickName == request.UserNickName);

            if(currentUser != null)
            {
                return BadRequest("User is already registated");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new RegistrUser()
            {
                UserNickName = request.UserNickName,
                UserName = request.UserName,
                UserLastName = request.UserLastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _registrationContext.RegistrUsers.Add(user);
            await _registrationContext.SaveChangesAsync();

            return Ok(currentUser);

        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserDTOLog request)
        {
            var user = _registrationContext.RegistrUsers.FirstOrDefault(x => x.UserNickName == request.UserNickName);

            if (user == null)
            {
                return NotFound("Not registred user");
            }

            if (!CheckPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Incorrect password");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<string>> Delete(string nickName)
        {
            using (var context = _registrationContext)
            {
                context.RegistrUsers.Remove(context.RegistrUsers.FirstOrDefault(ru => ru.UserNickName == nickName));
                _registrationContext.SaveChanges();
            }

            return Ok();
        }

        private string CreateToken(RegistrUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
            
        }

        //хеширование пароля
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())  //использование метода хеширования
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
