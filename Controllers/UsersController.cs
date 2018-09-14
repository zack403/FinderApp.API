using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Model;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinderApp.API.Controllers
{
    [Authorize]
    [Route("api/user")]
    public class UsersController : Controller
    {
        private readonly FinderDbContext context;
        private readonly IFinderRepository repository;
        private readonly IMapper mapper;
        public UsersController(IFinderRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await repository.GetUsers();
            var userToReturn = mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userToReturn);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetUsersById(int id)
        {
            var user = await repository.GetUser(id);
            var userToreturn = mapper.Map<UserDetailedDto>(user);
            return Ok(userToreturn);

        }

    }
}