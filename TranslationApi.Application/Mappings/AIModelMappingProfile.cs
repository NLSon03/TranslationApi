using AutoMapper;
using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Mappings
{
    public class AIModelMappingProfile : Profile
    {
        public AIModelMappingProfile()
        {
            // List DTO mapping
            CreateMap<AIModel, AIModelDto>();

            // Detail DTO mapping (inherits from List DTO)
            CreateMap<AIModel, AIModelDto>();

            // Create DTO mapping
            CreateMap<AIModelDto, AIModel>();

            // Update DTO mapping
            CreateMap<AIModelDto, AIModel>();
        }
    }
}