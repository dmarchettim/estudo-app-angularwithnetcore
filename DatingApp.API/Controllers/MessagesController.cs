using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LoginUserActivity))] //para utilizar o Filter que criamos
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private IDatingRepository _repository;
        public IMapper _mapper;
        public MessagesController(IDatingRepository _repository, IMapper _mapper)
        {
            this._mapper = _mapper;
            this._repository = _repository;
        }

        [HttpGet("{messageId}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int messageId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repository.GetMessage(messageId);

            if(messageFromRepo == null)
                return NotFound();
            
            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForuser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            messageParams.UserId = userId;

            var messagesFromRepo = await _repository.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messagesFromRepo);

            //adicionando paginação através do extended method para retornar isso ao front e entao paginar na tela
            Response.AdicionarPaginacao(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                    messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            
            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _repository.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messageFromRepo);
            
            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDTO messageForCreationDTO)
        {
            var sender = await _repository.GetUser(userId);
            
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var recipient = await _repository.GetUser(messageForCreationDTO.RecipientId);

            messageForCreationDTO.SenderId = userId;

            if(recipient == null)
                return BadRequest("Não foi possível encontrar o recipiente com ID " + messageForCreationDTO.RecipientId);

            var message = _mapper.Map<Message>(messageForCreationDTO);

            _repository.Add(message);
                        
            if(await _repository.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDTO>(message);
                Console.WriteLine(message.Id);
                return CreatedAtRoute("GetMessage", new {userId, messageId = message.Id}, messageToReturn); // atencao: no .NETCore 3.0 precisa fornecer tbm o userId
            }

            throw new System.Exception("Falha ao criar a mensagem");
        }

        [HttpPost("{messageId}")] 
        // nao vamos usar o HttpDelete porque não vamos de fato deletar a mensagem, senao sairá também da caixa da outra ponta
        public async Task<IActionResult> DeleteMessage(int messageId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repository.GetMessage(messageId);

            if(messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;
            
            if(messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeteled = true;
            
            if(messageFromRepo.RecipientDeteled && messageFromRepo.SenderDeleted)
                _repository.Delete(messageFromRepo);
            
            if(await _repository.SaveAll())
                return NoContent();
            
            throw new Exception("Error deleting the message");
            
        }

        [HttpPost("{messageId}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var message = await _repository.GetMessage(messageId);

            if(message.RecipientId != userId)
                return Unauthorized();
            
            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repository.SaveAll();

            return NoContent();
        }
    }
}