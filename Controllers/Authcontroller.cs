using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Helpers;
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
        private readonly IMapper mapper;
        public Authcontroller(IUnitOfWork unitofwork, IAuthRepository authrepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.authrepository = authrepository;
            this.unitofwork = unitofwork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userregisterdto)
        {
            if (!string.IsNullOrEmpty(userregisterdto.Username))
                userregisterdto.Username = userregisterdto.Username.ToLower();

            if (await authrepository.UserExists(userregisterdto.Username))
                return BadRequest("username already taken, Try a different username");

            //validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usertocreate = mapper.Map<User>(userregisterdto);

            var createduser = await authrepository.Register(usertocreate, userregisterdto.Password);
            var userToReturn = mapper.Map<UserDetailedDto>(createduser);


            return CreatedAtRoute("GetUser", new {message = "Account created successfully",controller = "user", id = usertocreate.Id}, userToReturn);

            //return Ok($"User {usertocreate.Username} successfully created!");



        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userlogindto)
        {
            var userFromRepo = await authrepository.Login(userlogindto.Username.ToLower(), userlogindto.Password);
            if (userFromRepo == null)
                return BadRequest("Login failed, username or password incorrect");

            //generate token
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("super secret key");
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                        new Claim(ClaimTypes.Name, userFromRepo.Username)

                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)

            };
            var token = tokenhandler.CreateToken(tokendescriptor);
            var tokenString = tokenhandler.WriteToken(token);

            var user = mapper.Map<UserDetailedDto>(userFromRepo);

            return Ok(new { message="Login was successful", tokenString, user });
        }




    }
}