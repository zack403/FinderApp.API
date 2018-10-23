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
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/user/{userId}/[controller]")]
    public class MessagesController : Controller
    {
        private readonly IFinderRepository repository;
        private readonly IMapper mapper;
        public MessagesController(IFinderRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await repository.GetMessage(id);
            if(messageFromRepo == null)
            return NotFound();

            return Ok(messageFromRepo);

        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messagesFromRepo = await repository.GetMessagesForUser(messageParams);
            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);            
            
        }


        [HttpGet("thread/{id}")]

        public async Task<IActionResult> GetMessageThread(int userId, int id)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await repository.GetMessageThread(userId, id);
            var messageThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            return Ok(messageThread);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody] MessageCreationDto messageCreationDto)
        {
        

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageCreationDto.SenderId = userId;

            var recipient = await repository.GetUser(messageCreationDto.RecipientId);
            var sender = await repository.GetUser(messageCreationDto.SenderId);

            if (recipient == null)
            return BadRequest($"Could not find user {messageCreationDto.RecipientId}");

            var message = mapper.Map<Message>(messageCreationDto);
            repository.Add(message);

            var messageToReturn = mapper.Map<MessageToReturnDto>(message);

            if(await repository.CompleteAsync())
            return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);

            throw new Exception("Creating message failed on save");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await repository.GetMessage(id);
            if(messageFromRepo.SenderId == userId)
            messageFromRepo.SenderDeleted = true;

            if(messageFromRepo.RecipientId == userId)
            messageFromRepo.RecipientDeleted = true;

            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            repository.Delete(messageFromRepo);


            if(await repository.CompleteAsync())

            return NoContent();

            throw new Exception("Error deleting the message");
        }


        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int id, int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var message = await repository.GetMessage(id);

            if(message.RecipientId != userId)
            return BadRequest("Failed to mark message as read");

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await repository.CompleteAsync();

            return NoContent();
        }
    }
}