namespace TranslationApi.Application.DTOs
{
    public class AIModelListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AIModelDetailDto : AIModelListDto
    {
        public string Config { get; set; } = string.Empty;
    }

    public class AIModelCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Config { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class AIModelUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Config { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}