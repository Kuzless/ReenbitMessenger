﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyMessenger.MApplication.DTO;
using MyMessenger.MApplication.DTO.AuthDTOs;
using MyMessenger.MApplication.СommandsQueries.Users.Commands;
using MyMessenger.MApplication.СommandsQueries.Users.Queries;

namespace MyMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IMediator mediator;
        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult<TokensDTO>> Login([FromBody] LoginDTO user)
        {
            var tokens = await mediator.Send(new LoginQuery(user));
            if (tokens.accessToken != null)
            {
                return Ok(tokens);
            } else
            {
                return BadRequest("Invalid login credentials");
            }
        }
        [HttpPost("refresh/")]
        public async Task<ActionResult<TokensDTO>> Refresh([FromBody] TokensDTO tokens)
        {
            var newTokens = await mediator.Send(new RefreshTokenQuery(tokens));
            if (newTokens.accessToken != null)
            {
                return Ok(newTokens);
            }
            else
            {
                return BadRequest("Tokens are expired!");
            }
        }
        [HttpPost("sign/")]
        public async Task<ActionResult<ResponseDTO>> SignUp([FromBody] SignUpDTO user)
        {
            var response = await mediator.Send(new SignUpCommand(user));
            return Ok(response);
        }

    }
}
