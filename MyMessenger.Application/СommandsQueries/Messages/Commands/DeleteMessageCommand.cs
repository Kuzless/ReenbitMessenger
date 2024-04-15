﻿using MediatR;

namespace MyMessenger.Application.СommandsQueries.Messages.Commands
{
    public class DeleteMessageCommand : IRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DeleteMessageCommand(int Id, string UserId)
        {
            this.Id = Id;
            this.UserId = UserId;
        }
    }
}
