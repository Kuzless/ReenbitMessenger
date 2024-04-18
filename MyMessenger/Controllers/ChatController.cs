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

namespace MyMessenger.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IMediator mediator;
        private readonly IJWTRetrievalService jWTRetrievalService;
        public ChatController(IMediator mediator, IJWTRetrievalService jWTRetrievalService)
        {
            this.jWTRetrievalService = jWTRetrievalService;
            this.mediator = mediator;
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<IActionResult> GetAllChats([FromQuery] AllDataRetrievalParametersDTO data, [FromHeader] string userAccessToken)
        {
            var userid = jWTRetrievalService.GetIdByToken(new TokensDTO() { accessToken = userAccessToken });
            var chats = await mediator.Send(new GetAllChatsQuery(data.Sort, data.PageNumber, data.PageSize, data.Subs, userid));
            return Ok(chats);
        }
        [HttpPost]
        public async Task CreateChat([FromBody] ChatDTO chat, [FromHeader] string userAccessToken)
        {
            var userid = jWTRetrievalService.GetIdByToken(new TokensDTO() { accessToken = userAccessToken });
            await mediator.Send(new CreateChatCommand(userid, chat.Name, userid));
        }
        [HttpPost("member/")]
        public async Task JoinChat([FromBody] ChatDTO chat, [FromHeader] string userAccessToken)
        {
            var userid = jWTRetrievalService.GetIdByToken(new TokensDTO() { accessToken = userAccessToken });
            await mediator.Send(new JoinChatCommand(userid, chat.Id));
        }
        [HttpDelete("{id}")]
        public async Task DeleteChat(int id, [FromHeader] string userAccessToken)
        {
            var userid = jWTRetrievalService.GetIdByToken(new TokensDTO() { accessToken = userAccessToken });
            await mediator.Send(new DeleteChatCommand(userid, id));
        }
        [HttpDelete("member/{id}")]
        public async Task LeaveChat(int id, [FromHeader] string userAccessToken)
        {
            var userid = jWTRetrievalService.GetIdByToken(new TokensDTO() { accessToken = userAccessToken });
            await mediator.Send(new LeaveChatCommand(userid, id));
        }
    }
}
