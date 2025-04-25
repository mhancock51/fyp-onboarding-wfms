using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.BusinessLogic.MediatRHandlers;
using OnboardingWFMSApi.BusinessLogic.TaskInstanceLogic;
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

        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IOnboardingEmployeeDetailsRepository _onboardingEmployeeDetailsRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;

        private readonly IFileUploadTaskInstanceRepository _uploadTaskInstanceRepository;

        private readonly IAccountLogic _accountLogic;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        private readonly IMapper _mapper;
        private readonly ILogger<DocumentLogic> _logger;
        private readonly IMediator _mediator;

        public DocumentLogic(IDocumentRepository documentRepository, ITaskInstanceRepository taskInstanceRepository, IMapper mapper,
            IAccountLogic accountLogic, ITaskInstanceLogic taskInstanceLogic, IDocumentAccessLinkRepository documentAccessLinkRepository, ILogger<DocumentLogic> logger,
            IWorkflowInstanceRepository workflowInstanceRepository, IOnboardingEmployeeDetailsRepository onboardingEmployeeDetailsRepository,
            IFileUploadTaskInstanceRepository uploadTaskInstanceRepository, IMediator mediator)
        {
            _documentRepository = documentRepository;
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _accountLogic = accountLogic;
            _taskInstanceLogic = taskInstanceLogic;
            _documentAccessLinkRepository = documentAccessLinkRepository;
            _logger = logger;
            _workflowInstanceRepository = workflowInstanceRepository;
            _onboardingEmployeeDetailsRepository = onboardingEmployeeDetailsRepository;
            _uploadTaskInstanceRepository = uploadTaskInstanceRepository;
            _mediator = mediator;
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
            var canAccess = await CheckAccessToDocument(accountId, documentId, doc.WorkflowInstanceId);
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
            DocumentTable document = null;
            try
            {
                document = new DocumentTable()
                {
                    TaskInstanceId = payload.TaskInstanceId,
                    CreatorId = accountId,
                    WorkflowInstanceId = workflowInstanceId,
                    FileExtension = Path.GetExtension(payload.FileName),
                    UploadTimestamp = DateTime.Now,
                    FileName = payload.DocumentName,
                    DocumentData = Convert.FromBase64String(payload.FileBase64)
                };
                document = await _documentRepository.AddAsync(document);
            }
            catch (Exception ex)
            {
                return new HTTPResponse<DocumentDTO, string>() { Success = false, HttpCode = 500, Error = "Failed to upload document" };
            }

            if (!string.IsNullOrEmpty(payload.TaskInstanceId))
            {
                // update task instance
                var uploadTaskData = await _uploadTaskInstanceRepository.GetByTaskInstanceId(payload.TaskInstanceId);
                uploadTaskData.DocumentId = document.Id;                
                try
                {
                    await _uploadTaskInstanceRepository.UpdateAsync(uploadTaskData);
                }
                catch(Exception ex)
                {
                    await _documentRepository.DeleteAsync(document);
                    return new HTTPResponse<DocumentDTO, string>() { Success = false, HttpCode = 400, Error = "Failed to update task instance" };
                }
            }

            // assign access to document
            await _documentAccessLinkRepository.AddAsync(new DocumentAccessLinkTable() { Id = "", AccountId = accountId, DocumentId = document.Id });
            foreach(var accessAccountId in payload.AccessAccountIds)
            {
                var link = await _documentAccessLinkRepository.AddAsync(new DocumentAccessLinkTable() { Id = "", AccountId = accessAccountId, DocumentId = document.Id });
                if (link == null) _logger.LogError($"Failed to provide access to document for account {accessAccountId}");
            }

            // create audit log            
            if (taskInstance?.WorkflowInstanceId != null)
            {
                var log = new CreateWorkflowInstanceAuditLogPayload()
                {
                    WorkflowInstanceId = taskInstance.WorkflowInstanceId,
                    Log = $"{document.FileName} uploaded",
                    AccountId = accountId
                };
                await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));
            }

            return new HTTPResponse<DocumentDTO, string>() { Success = true, HttpCode = 200, Data = _mapper.Map<DocumentDTO>(document) };
        }

        private async Task<bool> CheckAccessToDocument(string accountId, string documentId, string? workflowInstanceId)
        {
            // attempt with actual account Id
            var access = await _documentAccessLinkRepository.GetAccountsAccessToResource(accountId, documentId);
            if (access != null) return true;

            // failed to gaina access with real account id, check if account is associated through workflow instance
            if (string.IsNullOrEmpty(workflowInstanceId))
            {
                return false;
            }
            var workflowInstance = await _workflowInstanceRepository.GetById(workflowInstanceId);
            // if account id is supervisor of workflow instance, replace account id with placeholder when doing look up
            if (accountId == workflowInstance.SupervisorAccountId)
            {
                accountId = AccountUtility.SUPERVISOR_ACCOUNT_ID_PLACEHOLDER;
                access = await _documentAccessLinkRepository.GetAccountsAccessToResource(accountId, documentId);
                if (access != null) return true;
            }

            var onboardersDetails = await _onboardingEmployeeDetailsRepository.GetDetailsByWorkflowInstance(workflowInstanceId);            
            // like so if onboarder
            if (accountId == onboardersDetails.OnboarderAccountId)
            {
                accountId = AccountUtility.ONBOARDER_ACCOUNT_ID_PLACEHOLDER;
                access = await _documentAccessLinkRepository.GetAccountsAccessToResource(accountId, documentId);
                return access != null;
            }
            return false;
        }
    }

}
