namespace weddingcraft_be.Models.Configuration
{
    public class RedisSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 6379;
        public string? Password { get; set; }
        public bool Ssl { get; set; } = false;
        public bool AbortConnect { get; set; } = false;
        public int ConnectTimeout { get; set; } = 5000;
        public int SyncTimeout { get; set; } = 5000;

        public string ToConnectionString()
        {
            var config = $"{Host}:{Port},abortConnect={AbortConnect},ssl={Ssl},connectTimeout={ConnectTimeout},syncTimeout={SyncTimeout}";
            if (!string.IsNullOrEmpty(Password))
            {
                config += $",password={Password}";
            }
            return config;
        }
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
