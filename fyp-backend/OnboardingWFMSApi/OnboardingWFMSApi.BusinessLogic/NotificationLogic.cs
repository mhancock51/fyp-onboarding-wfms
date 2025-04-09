using AutoMapper;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface INotificationLogic
    {
        public Task<ServerResponse<string, string>> CreateNotification(CreateNotificationPayload payload);
        public Task<NotificationDTO> GetNotificationDTO(string notificationId);
        public Task<HTTPResponse<List<NotificationDTO>, string>> GetAccountsNotification(string accountId);
        public Task<HTTPResponse<string, string>> DeleteNotificaion(string notificationId, string accountId);
    }
    public class NotificationLogic : INotificationLogic
    {
        public const string NOTIFICATION_UNSEEN_STATUS = "unseen";
        public const string NOTIFICATION_SEEN_STATUS = "seen";

        private readonly INotificationRepository _notificationRepository;

        private readonly IMapper _mapper;
        private readonly ILogger<NotificationLogic> _logger;

        public NotificationLogic(INotificationRepository notificationRepository, ILogger<NotificationLogic> logger, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ServerResponse<string, string>> CreateNotification(CreateNotificationPayload payload)
        {
            var notification = new NotificationTable()
            {
                Id = "",
                RecipientId = payload.RecipientId,
                Description = payload.Description,
                Status = NOTIFICATION_UNSEEN_STATUS,
                Tags = payload.Tags,
                Timestamp = DateTime.Now
            };
            try
            {
                await _notificationRepository.AddAsync(notification);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create notification: {ex.Message}");
                return new ServerResponse<string, string>() { Success = false };
            }
        }

        public async Task<HTTPResponse<string, string>> DeleteNotificaion(string notificationId, string accountId)
        {
            var notification = await _notificationRepository.GetById(notificationId);
            if (notification == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Notification doesn't exist" };
            }
            if (notification.RecipientId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "User doesn't have permisssion to do this" };
            }
            if (notification.Status != NOTIFICATION_SEEN_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Notification must have been seen by the recipient" };
            }
            try
            {
                await _notificationRepository.DeleteAsync(notification);
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully deleted notificiation" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete notification: {ex.Message}");
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to delete notification" };
            }
        }

        public async Task<HTTPResponse<List<NotificationDTO>, string>> GetAccountsNotification(string accountId)
        {
            var notificationIds = (await _notificationRepository.GetAccountsNotifications(accountId)).Select(n => n.Id).ToList();
            var notificationDTOs = new List<NotificationDTO>();
            foreach (var id in notificationIds)
            {
                var dto = await GetNotificationDTO(id);
                if (dto != null) 
                { 
                    notificationDTOs.Add(dto); 
                }
            }
            return new HTTPResponse<List<NotificationDTO>, string>() { Success = true, Data = notificationDTOs, HttpCode = 200 };
        }

        public async Task<NotificationDTO> GetNotificationDTO(string notificationId)
        {
            var notification = await _notificationRepository.GetById(notificationId);
            if (notification == null) return null;

            // if status is unseen, mark it as seen
            string previousStatus = notification.Status;
            if (notification.Status == NOTIFICATION_UNSEEN_STATUS)
            {                
                notification.Status = NOTIFICATION_SEEN_STATUS;
                try
                {
                    await _notificationRepository.UpdateAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to mark unseen notification as seen");
                }
                notification.Status = previousStatus;
            }

            var notificationDTO = _mapper.Map<NotificationDTO>(notification);
            return notificationDTO;
        }
    }
}
