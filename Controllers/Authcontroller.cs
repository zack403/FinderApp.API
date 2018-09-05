using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Model;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace FinderApp.API.Controllers
{
    [Route("/api/auth")]
    public class Authcontroller : Controller
    {
        private readonly IUnitOfWork unitofwork;
        private readonly IAuthRepository authrepository;
        public Authcontroller(IUnitOfWork unitofwork, IAuthRepository authrepository)
        {
            this.authrepository = authrepository;
            this.unitofwork = unitofwork;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userregisterdto)
        {
            userregisterdto.Username = userregisterdto.Username.ToLower();
            if (await authrepository.UserExists(userregisterdto.Username))
                ModelState.AddModelError("Username", "already exist Try a different username");

            //validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var usertocreate = new User
            {
                Username = userregisterdto.Username
            };

            var createduser = await authrepository.Register(usertocreate, userregisterdto.Password);
            return Ok($"User {usertocreate.Username} successfully created!");


        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userlogindto)
        {

            var credentials = await authrepository.Login(userlogindto.Username.ToLower(), userlogindto.Password);
            if (credentials == null)
                return Unauthorized();

            //generate token
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("super secret key");
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.NameIdentifier, credentials.Id.ToString()),
                        new Claim(ClaimTypes.Name, credentials.Username)

                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)

            };
            var token = tokenhandler.CreateToken(tokendescriptor);
            var tokenString = tokenhandler.WriteToken(token);

            return Ok(new { tokenString });

        }




    }
}