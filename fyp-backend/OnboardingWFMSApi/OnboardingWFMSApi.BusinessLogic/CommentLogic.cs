using AutoMapper;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
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
    public interface ICommentLogic
    {
        public Task<HTTPResponse<string, string>> CreateComment(CreateCommentPayload payload, string accountId);
        public Task<HTTPResponse<List<CommentDTO>, string>> GetTaskTemplateComments(string taskTemplateId);
        public Task<CommentDTO> GetCommentDTO(string commentId);
    }

    public class CommentLogic : ICommentLogic
    {
        private readonly IMapper _mapper;

        private readonly ICommentRepository _commentRepository;
        private readonly IAccountLogic _accountLogic;

        public CommentLogic(ICommentRepository commentRepository, IMapper mapper, IAccountLogic accountLogic)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _accountLogic = accountLogic;
        }

        public async Task<HTTPResponse<string, string>> CreateComment(CreateCommentPayload payload, string accountId)
        {
            if (string.IsNullOrEmpty(payload.ParentCommentId)) payload.ParentCommentId = null;
            try
            {
                await _commentRepository.AddAsync(new CommentTable()
                {
                    Id = "",
                    CommenterId = accountId,
                    Text = payload.Text,
                    TaskTemplateId = payload.TaskTemplateId,
                    CreationTimestamp = DateTime.Now,
                    ParentCommentId = payload.ParentCommentId
                });
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created comment" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to create comment" };
            }
        }

        public async Task<CommentDTO> GetCommentDTO(string commentId)
        {
            var comment = await _commentRepository.GetById(commentId);
            var dto = _mapper.Map<CommentDTO>(comment);
            dto.AccountDirectory = await _accountLogic.GetDirectoryByAccountId(comment.CommenterId);
            return dto;
        }

        public async Task<HTTPResponse<List<CommentDTO>, string>> GetTaskTemplateComments(string taskTemplateId)
        {
            var comments = await _commentRepository.GetCommentsByTaskTemplateId(taskTemplateId);
            var commentDtos = new List<CommentDTO>();
            foreach(var comment in comments)
            {
                commentDtos.Add(await GetCommentDTO(comment.Id));
            }
            return new HTTPResponse<List<CommentDTO>, string>() { Success = true, HttpCode = 200, Data = commentDtos };
        }
    }
}
