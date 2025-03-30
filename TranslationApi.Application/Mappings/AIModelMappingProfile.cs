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
            CreateMap<AIModel, AIModelListDto>();

            // Detail DTO mapping (inherits from List DTO)
            CreateMap<AIModel, AIModelDetailDto>();

            // Create DTO mapping
            CreateMap<AIModelCreateDto, AIModel>();

            // Update DTO mapping
            CreateMap<AIModelUpdateDto, AIModel>();
        }
    }
}