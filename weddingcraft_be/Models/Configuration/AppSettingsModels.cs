namespace weddingcraft_be.Models.Configuration
{
    public class RedisSettings
    {
        public string Configuration { get; set; } = null!;
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string User { get; set; } = null!;
        public string Pass { get; set; } = null!;
        public string From { get; set; } = null!;
    }

    public class GeminiSettings
    {
        public string ApiKey { get; set; } = null!;
    }
}
