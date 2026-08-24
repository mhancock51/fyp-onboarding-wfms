using MediatR;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.BusinessLogic.NotificationLogic;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.MediatRHandlers
{
    public class CreateNotificationRequest : IRequest<ServerResponse<string, string>>
    {
        public CreateNotificationRequest(CreateNotificationPayload payload)
        {
            this.payload = payload;
        }

        public CreateNotificationPayload payload { get; set; }
    }

    public class CreateNotificationHandler : IRequestHandler<CreateNotificationRequest, ServerResponse<string, string>>
    {
        private readonly INotificationLogic _notificationLogic;

        public CreateNotificationHandler(INotificationLogic notificationLogic)
        {
            _notificationLogic = notificationLogic;
        }

        public async Task<ServerResponse<string, string>> Handle(CreateNotificationRequest request, CancellationToken cancellationToken)
        {
            return await _notificationLogic.CreateNotification(request.payload);
        }
    }
}
