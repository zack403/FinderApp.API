using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userupdateDto)
        {   if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await repository.GetUser(id);
            if(userFromRepo == null)
            return NotFound($"The user with ID {id} does not exist");

            if(currentUserId != userFromRepo.Id)
            return Unauthorized();
            mapper.Map(userupdateDto, userFromRepo);

            if (await repository.CompleteAsync())
            return NoContent();
            throw new Exception($"updating user {id} failed on save");
        }

    }
}