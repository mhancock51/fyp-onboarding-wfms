using Microsoft.AspNetCore.Http;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IDocumentLogic
    {
        public Task<HTTPResponse<string, string>> UploadDocument(UploadDocumentPayload payload, string accountId);
    }
    public class DocumentLogic : IDocumentLogic
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        public DocumentLogic(IDocumentRepository documentRepository, ITaskInstanceLogic taskInstanceLogic)
        {
            _documentRepository = documentRepository;
            _taskInstanceLogic = taskInstanceLogic;
        }

        public async Task<HTTPResponse<string, string>> UploadDocument(UploadDocumentPayload payload, string accountId)
        {
            // check task instance exists and is open
            var response = await _taskInstanceLogic.GetTaskInstance(payload.TaskInstanceId);
            if (response.Success == false || response.Data == null) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            // check task instance is an "upload doc" task


            // check account is valid
            // check workflow exists and is open

            // check contents of file
            // check file extension matches onces allow in task template
            try
            {
                var document = new DocumentTable()
                {
                    TaskInstanceId = "",
                    CreatorId = accountId,
                    WorkflowInstanceId = "",
                    FileExtension = Path.GetExtension(payload.file.FileName),
                    UploadTimestamp = DateTime.Now,
                    FileName = "file"
                };
                await _documentRepository.AddAsync(document);
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully uploaded document" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to upload document" };
            }

            // TODO: update task instance to mark as complete and update metadata
        }
    }

}
