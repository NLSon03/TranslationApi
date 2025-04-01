
using AutoMapper;
using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Mappings
{
    public class AIModelMappingProfile : Profile
    {
        public AIModelMappingProfile()
        {
            CreateMap<AIModel, AIModelListDto>();
            CreateMap<AIModel, AIModelDetailDto>();
            
            CreateMap<AIModelCreateDto, AIModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ChatSessions, opt => opt.Ignore());

            CreateMap<AIModelUpdateDto, AIModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ChatSessions, opt => opt.Ignore());
        }
    }
}