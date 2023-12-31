﻿using Lifetrons.Erp.Web.Models;
using Lifetrons.Erp.Web.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Lifetrons.Erp.Web.Domain.Twilio.MarketingNotifications
{
    public class SubscribersNotificationMessageCreator
    {
        private readonly ISubscribersRepository _repository;
        private const string Subscribe = "add";
        private const string Unsubscribe = "remove";

        public SubscribersNotificationMessageCreator(ISubscribersRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Create(string phoneNumber, string message)
        {
            var subscriber = await _repository.FindByPhoneNumberAsync(phoneNumber);
            if (subscriber != null)
            {
                return await CreateOutputMessage(subscriber, message.ToLower());
            }

            subscriber = new Subscriber
            {
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _repository.CreateAsync(subscriber);
            return "Thanks for contacting TWBC! Text 'add' if you would to receive updates via text message.";
        }

        private async Task<string> CreateOutputMessage(Subscriber subscriber, string message)
        {
            if (!IsValidCommand(message))
            {
                return "Sorry, we don't recognize that command. Available commands are: 'add' or 'remove'.";
            }

            var isSubscribed = message.StartsWith(Subscribe);
            subscriber.Subscribed = isSubscribed;
            subscriber.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(subscriber);

            return isSubscribed
                ? "You are now subscribed for updates."
                : "You have unsubscribed from notifications. Test 'add' to start receiving updates again";
        }

        private static bool IsValidCommand(string command)
        {
            return command.StartsWith(Subscribe) || command.StartsWith(Unsubscribe);
        }
    }
}