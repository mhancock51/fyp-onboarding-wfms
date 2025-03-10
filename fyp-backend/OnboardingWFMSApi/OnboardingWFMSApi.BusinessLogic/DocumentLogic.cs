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
        public Task<HTTPResponse<DocumentTable, string>> UploadDocument(UploadDocumentPayload payload, string accountId);
        public Task<HTTPResponse<DocumentTable, string>> GetDocument(string documentId);
    }
    public class DocumentLogic : IDocumentLogic
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentLogic(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<byte[]> ConvertIFormFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<HTTPResponse<DocumentTable, string>> GetDocument(string documentId)
        {
            var doc = await _documentRepository.GetById(documentId);
            return new HTTPResponse<DocumentTable, string>() { Success = true, HttpCode = 200, Data = doc };
        }

        public async Task<HTTPResponse<DocumentTable, string>> UploadDocument(UploadDocumentPayload payload, string accountId)
        {            
            // check task instance is an "upload doc" task


            // check account is valid
            // check workflow exists and is open

            // check contents of file
            // check file extension matches onces allow in task template
            try
            {
                var document = new DocumentTable()
                {
                    TaskInstanceId = payload.TaskInstanceId,
                    CreatorId = accountId,
                    WorkflowInstanceId = "",
                    FileExtension = Path.GetExtension(payload.File.FileName),
                    UploadTimestamp = DateTime.Now,
                    FileName = "file",
                    DocumentData = await ConvertIFormFileToByteArray(payload.File)
                };
                await _documentRepository.AddAsync(document);
                return new HTTPResponse<DocumentTable, string>() { Success = true, HttpCode = 200, Data = document };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<DocumentTable, string>() { Success = false, HttpCode = 500, Error = "Failed to upload document" };
            }

            // TODO: update task instance to mark as complete and update metadata
        }
    }

}
