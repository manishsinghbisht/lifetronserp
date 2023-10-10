﻿using Lifetrons.Erp.Web.DBContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Lifetrons.Erp.Web.Models.Repository
{
    public interface ISubscribersRepository
    {
        Task<List<Subscriber>> FindActiveSubscribersAsync();
        Task<Subscriber> FindByPhoneNumberAsync(string phoneNumber);
        Task<int> CreateAsync(Subscriber subscriber);
        Task<int> UpdateAsync(Subscriber subscriber);
    }

    public class SubscribersRepository : ISubscribersRepository
    {
        private readonly MarketingNotificationsContext _context;

        public SubscribersRepository()
        {
            _context = new MarketingNotificationsContext();
        }

        public async Task<List<Subscriber>> FindActiveSubscribersAsync()
        {
            return await _context.Subscribers
                .Where(s => s.Subscribed)
                .ToListAsync();
        }

        public async Task<Subscriber> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Subscribers.FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber);
        }

        public async Task<int> CreateAsync(Subscriber subscriber)
        {
            _context.Subscribers.Add(subscriber);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Subscriber subscriber)
        {
            _context.Entry(subscriber).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }
    }
}