﻿using MediatR;
using MyMessenger.Domain.Entities;

namespace MyMessenger.Application.СommandsQueries.Users.Queries
{
    public class GetUserByIdQuery : IRequest<User>
    {
        public string UserId { get; set; }
        public GetUserByIdQuery(string userId)
        {

            UserId = userId;

        }
    }
}
