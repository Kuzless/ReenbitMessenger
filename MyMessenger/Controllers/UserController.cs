﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyMessenger.MApplication.DTO;
using MyMessenger.MApplication.СommandsQueries.Users.Queries;

namespace MyMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] AllDataRetrievalParametersDTO data)
        {
            var users = await mediator.Send(new GetAllUsersQuery(data.Sort, data.PageNumber, data.PageSize, data.Subs));
            return Ok(users);
        }
    }
}