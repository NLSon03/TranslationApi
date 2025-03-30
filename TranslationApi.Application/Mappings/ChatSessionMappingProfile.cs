
using AutoMapper;
using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Mappings
{
    public class ChatSessionMappingProfile : Profile
    {
        public ChatSessionMappingProfile()
        {
            CreateMap<ChatSession, ChatSessionDto>()
                .ForMember(dest => dest.AIModelName,
                    opt => opt.MapFrom(src => src.AIModel.Name));

            CreateMap<ChatSession, ChatSessionDetailDto>()
                .ForMember(dest => dest.AIModelName,
                    opt => opt.MapFrom(src => src.AIModel.Name))
                .ForMember(dest => dest.Messages,
                    opt => opt.MapFrom(src => src.Messages));

            CreateMap<ChatMessage, ChatMessageDto>();
        }
    }
}