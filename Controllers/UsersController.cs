using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Helpers;
using FinderApp.API.Model;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinderApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
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
        public async Task<IActionResult> GetUsers(UserParams userparams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await repository.GetUser(currentUserId);

            userparams.userId = currentUserId;
            if(string.IsNullOrEmpty(userparams.Gender))
            {
                userparams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await repository.GetUsers(userparams);
            var userToReturn = mapper.Map<IEnumerable<UserDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(userToReturn);
        }

        [HttpGet("{id}", Name = "GetUSer")]
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


        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var like = await repository.GetLike(id, recipientId);

            if (like != null)
            return BadRequest("You already liked this user");
            
            if(await repository.GetUser(recipientId) == null)
            return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

             repository.Add<Like>(like);
             if(await repository.CompleteAsync())
             return Ok(new {});

             return BadRequest("Failed to add user");
        }
    }
}