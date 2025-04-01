
namespace TranslationApi.Application.DTOs
{
    public class AIModelListDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Version { get; set; }
        public bool IsActive { get; set; }
        public required string ModelType { get; set; }
        public required string Provider { get; set; }
    }

    public class AIModelDetailDto : AIModelListDto
    {
        public required string Config { get; set; }
    }

    public class AIModelCreateDto
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
        public bool IsActive { get; set; }
        public required string ModelType { get; set; }
        public required string Provider { get; set; }
        public required string Config { get; set; }
    }

    public class AIModelUpdateDto
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
        public bool IsActive { get; set; }
        public required string ModelType { get; set; }
        public required string Provider { get; set; }
        public required string Config { get; set; }
    }
}