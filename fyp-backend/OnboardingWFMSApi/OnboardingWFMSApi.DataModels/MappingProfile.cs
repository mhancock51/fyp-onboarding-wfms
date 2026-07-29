using AutoMapper;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<AccountTable, AuthenticatedAccountDTO>();
            CreateMap<AccountTable, InvitedAccountDTO>();
            CreateMap<AccountTable, AccountDirectoryDTO>();
            CreateMap<TaskTemplateTable, TaskTemplate>();
            CreateMap<TaskInstanceTable, TaskInstanceDTO>();
            CreateMap<TaskTypeTable, TaskType>();

            CreateMap<WorkflowTemplateTable, WorkflowTemplateDTO>();
            CreateMap<WorkflowTemplateNodeTable, WorkflowTemplateNodeDTO>();

            CreateMap<WorkflowInstanceTable, WorkflowInstanceDTO>();

            CreateMap<CommentTable, CommentDTO>();

            CreateMap<DocumentTable, DocumentDTO>();

            CreateMap<OnboardingEmployeeDetailsTable, OnboardingEmployeeDetailsDTO>();

            CreateMap<ReportedIssueTable, IssueDTO>();
            CreateMap<NotificationTable, NotificationDTO>();
            CreateMap<SubscriptionTierEntitlementTable, SubscriptionTierDTO>();

            CreateMap<TenantSubscriptionTable, TenantSubscriptionDTO>()
                .ForMember(dest => dest.SubscriptionCurrentPeriodEnd, opt => opt.MapFrom(src => src.StripeCurrentPeriodEnd))
                .ForMember(dest => dest.StripeSubscriptionStatus, opt => opt.MapFrom(src => src.StripeSubscriptionStatus))
                .ForMember(dest => dest.subscriptionTier, opt => opt.Ignore());

            CreateMap<CreateWorkflowTemplateNode, WorkflowTemplateNodeTable>();
            CreateMap<CreateWorkflowTemplatePayload, WorkflowTemplateTable>();
        }
    }
}
