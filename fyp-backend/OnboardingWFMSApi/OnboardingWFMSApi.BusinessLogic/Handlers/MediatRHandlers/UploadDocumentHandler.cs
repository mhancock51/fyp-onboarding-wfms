using MediatR;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.MediatRHandlers
{
    public class UploadDocumentRequest : IRequest<HTTPResponse<DocumentDTO, string>>
    {
        public UploadDocumentPayload payload;
        public string accountId;
        public UploadDocumentRequest(UploadDocumentPayload payload, string accountId)
        {
            this.payload = payload;
            this.accountId = accountId;
        }
    }

    public class UploadDocumentHandler : IRequestHandler<UploadDocumentRequest, HTTPResponse<DocumentDTO, string>>
    {
        private readonly IDocumentLogic _documentLogic;

        public UploadDocumentHandler(IDocumentLogic documentLogic)
        {
            _documentLogic = documentLogic;
        }

        public async Task<HTTPResponse<DocumentDTO, string>> Handle(UploadDocumentRequest request, CancellationToken cancellationToken)
        {
            return await _documentLogic.UploadDocument(request.payload, request.accountId);
        }
    }
}
