﻿using MediatR;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMessenger.Application.DTO;
using MyMessenger.Application.DTO.AuthDTOs;
using MyMessenger.Application.DTO.ChatDTOs;
using MyMessenger.Application.Services.JwtAuth.Interfaces;
using MyMessenger.Application.СommandsQueries.Chats.Commands;
using MyMessenger.Application.СommandsQueries.Chats.Queries;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;

namespace MyMessenger.Controllers
{
    [DisableCors]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IMediator mediator;
        public ChatController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllChats([FromQuery] AllDataRetrievalParametersDTO data)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var chats = await mediator.Send(new GetAllChatsQuery(data.Sort, data.PageNumber, data.PageSize, data.Subs, userid));
            return Ok(chats);
        }
        [HttpPost]
        public async Task CreateChat([FromBody] ChatDTO chat)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await mediator.Send(new CreateChatCommand(userid, chat.Name, userid));
        }
        [HttpPost("member/{username}")]
        public async Task JoinChat([FromBody] ChatDTO chat, string username)
        {
            await mediator.Send(new JoinChatCommand(username, chat.Id));
        }
        [HttpDelete("{id}")]
        public async Task DeleteChat(int id)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await mediator.Send(new DeleteChatCommand(userid, id));
        }
        [HttpDelete("member/{chatid}")]
        public async Task LeaveChat(int chatId)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await mediator.Send(new LeaveChatCommand(userid, chatId));
        }
    }
}
