using AutoMapper;
using OnboardingWFMSApi.DataModels.DTOs;
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
        }
    }
}
