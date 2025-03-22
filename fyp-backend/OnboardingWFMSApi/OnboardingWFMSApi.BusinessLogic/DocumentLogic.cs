using AutoMapper;
using Microsoft.AspNetCore.Http;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IDocumentLogic
    {
        public Task<HTTPResponse<DocumentDTO, string>> UploadDocument(UploadDocumentPayload payload, string accountId);
        public Task<HTTPResponse<DocumentDTO, string>> GetDocument(string documentId, string accountId);
        public Task<HTTPResponse<List<DocumentDTO>, string>> GetDocumentsFromWorkflowInstance(string workflowInstanceId, string accountId);
    }
    public class DocumentLogic : IDocumentLogic
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentAccessLinkRepository _documentAccessLinkRepository;

        private readonly ITaskInstanceRepository _taskInstanceRepository;

        private readonly IAccountLogic _accountLogic;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        private readonly IMapper _mapper;

        public DocumentLogic(IDocumentRepository documentRepository, ITaskInstanceRepository taskInstanceRepository, IMapper mapper, 
            IAccountLogic accountLogic, ITaskInstanceLogic taskInstanceLogic, IDocumentAccessLinkRepository documentAccessLinkRepository)
        {
            _documentRepository = documentRepository;
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _accountLogic = accountLogic;
            _taskInstanceLogic = taskInstanceLogic;
            _documentAccessLinkRepository = documentAccessLinkRepository;
        }

        public async Task<byte[]> ConvertIFormFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<HTTPResponse<DocumentDTO, string>> GetDocument(string documentId, string accountId)
        {
            var doc = await _documentRepository.GetById(documentId);
            // implement access control
            var canAccess = await CheckAccessToDocument(documentId, accountId);
            if (!canAccess)
            {
                return new HTTPResponse<DocumentDTO, string>() { Success = false, HttpCode = 403, Error = "Account does not have access to this document" };
            }

            var docDTO = _mapper.Map<DocumentDTO>(doc);
            docDTO.CreatorsAccount = await _accountLogic.GetDirectoryByAccountId(docDTO.CreatorId);
            docDTO.TaskInstance = (await _taskInstanceLogic.GetTaskInstance(docDTO.TaskInstanceId)).Data ?? null;

            return new HTTPResponse<DocumentDTO, string>() { Success = true, HttpCode = 200, Data = docDTO };
        }

        public async Task<HTTPResponse<List<DocumentDTO>, string>> GetDocumentsFromWorkflowInstance(string workflowInstanceId, string accountId)
        {
            var documentIds = (await _documentRepository.GetDocumentsFromWorkflowInstance(workflowInstanceId)).Select(d => d.Id);
            var documents = new List<DocumentDTO>();    
            foreach(var documentId in documentIds)
            {
                var response = await GetDocument(documentId, accountId);
                if (response.Success && response.HasData)
                {
                    documents.Add(response.Data);
                }
            }
            return new HTTPResponse<List<DocumentDTO>, string>() { Success = true, HttpCode = 200, Data = documents };
        }

        public async Task<HTTPResponse<DocumentDTO, string>> UploadDocument(UploadDocumentPayload payload, string accountId)
        {
            var taskInstance = await _taskInstanceRepository.GetById(payload.TaskInstanceId);
            string workflowInstanceId = null;
            if (taskInstance != null)
            {
                // check task instance is an "upload doc" task                

                workflowInstanceId = taskInstance.WorkflowInstanceId;
            }
            
            

            // check account is valid
            // check workflow exists and is open

            // check contents of file
            // check file extension matches onces allow in task template

            // TODO ensure document with same name isn't assigned to this workflow or task instance already
            try
            {
                var document = new DocumentTable()
                {
                    TaskInstanceId = payload.TaskInstanceId,
                    CreatorId = accountId,
                    WorkflowInstanceId = workflowInstanceId,
                    FileExtension = Path.GetExtension(payload.File.FileName),
                    UploadTimestamp = DateTime.Now,
                    FileName = payload.DocumentName,
                    DocumentData = await ConvertIFormFileToByteArray(payload.File)
                };
                await _documentRepository.AddAsync(document);
                return new HTTPResponse<DocumentDTO, string>() { Success = true, HttpCode = 200, Data = _mapper.Map<DocumentDTO>(document) };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<DocumentDTO, string>() { Success = false, HttpCode = 500, Error = "Failed to upload document" };
            }

            // TODO: update task instance to mark as complete and update metadata
        }

        private async Task<bool> CheckAccessToDocument(string accountId, string documentId)
        {
            var access = await _documentAccessLinkRepository.GetAccountsAccessToResource(accountId, documentId);
            return access != null;
        }
    }

}
