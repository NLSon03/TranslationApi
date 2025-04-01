namespace TranslationApi.API.Configurations
{
    public static class CorsConfiguration
    {
        public static void AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = new CorsSettings();
            configuration.GetSection("CorsSettings").Bind(corsSettings);

            services.AddCors(options =>
            {
                options.AddPolicy(corsSettings.PolicyName, policy =>
                {
                    policy
                    .WithOrigins(corsSettings.AllowedOrigins)
                    .WithMethods(corsSettings.AllowedMethods)
                    .WithHeaders(corsSettings.AllowedHeaders);
                });
            });
        }
    }
    public class CorsSettings
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string PolicyName { get; set; }
        public string[] AllowedOrigins { get; set; }
        public string[] AllowedMethods { get; set; }
        public string[] AllowedHeaders { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }

}
