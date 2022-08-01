using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _Tokenservice;

        public AccountController(DataContext context, ITokenService Tokenservice)
        {   
            _context = context;
            _Tokenservice = Tokenservice;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto register)
        {
           if(await UserExists(register.UserName))
            {
                return BadRequest("UserName Already exist");
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = register.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>>login(LoginDto login)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName==login.UserName);
            if (user == null) 
                return  Unauthorized("Invalid UserName");
            var hmac=new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }
            return new UserDto
            {
                UserName=user.UserName,
                Token = _Tokenservice.CreateToken(user)
            };
        }


        private async Task<bool>UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
