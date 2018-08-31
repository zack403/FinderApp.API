using System.Threading.Tasks;
using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Model;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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



    }
}