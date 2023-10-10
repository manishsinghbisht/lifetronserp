using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using Lifetrons.Erp.Web.Models.Repository;
using Lifetrons.Erp.Web.ViewModel;
using Lifetrons.Erp.Web.Domain.Twilio.MarketingNotifications;
using Lifetrons.Erp.Web.Domain.Twilio.Notifications;
using Twilio.Rest.Api.V2010.Account;

namespace Lifetrons.Erp.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ISubscribersRepository _repository;
        private readonly ISubscribersNotificationService _notificationService;

        public NotificationsController() : this(new SubscribersRepository(), new SubscribersNotificationService())
        {
        }

        public NotificationsController(ISubscribersRepository repository, ISubscribersNotificationService notificationService)
        {
            _notificationService = notificationService;
            _repository = repository;
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Works from Database Subscriber table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(NotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    //MMS
                    var mediaUrl = new List<Uri> { new Uri(model.ImageUrl) };
                    var subscribers = await _repository.FindActiveSubscribersAsync();
                    foreach (var subscriber in subscribers)
                    {
                        await _notificationService.SendMessageAsync(
                            subscriber.PhoneNumber,
                            model.Message,
                            mediaUrl);
                    }
                }
                else
                {
                    //SMS
                    var subscribers = await _repository.FindActiveSubscribersAsync();
                    foreach (var subscriber in subscribers)
                    {
                        await _notificationService.SendMessageAsync(
                            subscriber.PhoneNumber,
                            model.Message);
                    }
                }


                ModelState.Clear();
                ViewBag.FlashMessage = "Messages on their way!";
                return View();
            }

            return View(model);
        }

        /// <summary>
        /// Works from CSV in App_DataFolder
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> SendSMSFromCsv()
        {
            return View("SendSMSFromCsv");
        }
        /// <summary>
        /// Works from CSV in App_DataFolder
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SendSMSFromCsv(string body)
        {
            ////Send message
            var message = string.Format(body);
            var notifier = new Notifier(new AdministratorsRepository());
            var smsResult = await notifier.SendMessagesAsync(message);

            foreach (var item in smsResult)
            {
                if (item.Status == MessageResource.StatusEnum.Failed)
                {
                    ViewBag.SMSMessage = item.To + " " + item.ErrorMessage;
                }
            }

            return View("SendSMSFromCsv");
        }
    }
}