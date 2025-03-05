using AutoMapper;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables;
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
            CreateMap<TaskTemplateTable, TaskTemplate>();
            CreateMap<TaskInstanceTable, TaskInstance>();
            CreateMap<TaskTypeTable, TaskType>();
        }
    }
}
