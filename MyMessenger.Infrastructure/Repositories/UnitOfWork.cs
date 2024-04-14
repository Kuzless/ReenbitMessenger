﻿using Microsoft.EntityFrameworkCore;
using MyMessenger.Domain.Interfaces;
using MyMessenger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessenger.Domain.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext context;
        private readonly Dictionary<Type, object> repos = new Dictionary<Type, object>();
        public IChatRepository Chat { get; private set; }

        public UnitOfWork(DatabaseContext context)
        {
            this.context = context;
            Chat = new ChatRepository(context);
            repos[typeof(Entities.Chat)] = Chat;
        }
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!repos.Keys.Contains(type))
            {
                repos[type] = new GenericRepository<TEntity>(context);
            }
            return (IGenericRepository<TEntity>)repos[type];
        }

        public async Task SaveAsync()
        {   
            await context.SaveChangesAsync();
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
